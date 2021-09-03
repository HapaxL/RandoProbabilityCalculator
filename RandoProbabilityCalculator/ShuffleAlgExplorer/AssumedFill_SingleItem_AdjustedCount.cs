using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class AssumedFill_SingleItem_AdjustedCount : Algorithm
    {
        int ocCount = 0;

        public override Dictionary<string, int> Shuffle(Outcome outcome)
        {
            Console.WriteLine("starting singleitem adjusted count");
            var compiled = new Dictionary<string, int>();
            Shuffle(compiled, outcome, new Dictionary<Location, Item>());
            Console.WriteLine("ending singleitem adjusted count");
            return compiled;
        }

        public void Shuffle(Dictionary<string, int> compiled, Outcome outcome, Dictionary<Location, Item> reserved)
        {
            var reservedLocations = new Dictionary<Location, Item>(reserved);

            // Console.WriteLine($"{outcome.UnplacedItems.Count} unplaced items");

            if (outcome.UnplacedItems.Count == 0 || outcome.EmptyLocations.Count == 0)
            {
                ocCount++;
                if (ocCount % 1000 == 0) Console.WriteLine(ocCount);
                CompileOutcome(ocCount, compiled, outcome);
                //if (ocCount == 682922)
                //{
                //    Console.WriteLine("problematic");
                //}
                return;
            }

            var reachableResults = new Dictionary<Item, List<Location>>();

            foreach (var item in outcome.UnplacedItems.Distinct())
            {
                var unplaced = new List<Item>(outcome.UnplacedItems);
                unplaced.Remove(item);
                var allItems = FetchAllItems(unplaced, outcome.World);
                var reachable = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems)).ToList();

                if (reachable.Count == 1)
                {
                    // newOutcome = newOutcome.WithItemInLocation(item, reachable[0]);
                    //foreach (var kvp in reachableResults)
                    //{
                    //    kvp.Value.Remove(reachable[0]);
                    //}
                    if (reservedLocations.ContainsKey(reachable[0]))
                    {
                        if (reservedLocations[reachable[0]] != item)
                        {
                            throw new Exception("what");
                        }
                    }
                    else
                    {
                        reservedLocations.Add(reachable[0], item);
                    }
                }

                reachableResults.Add(item, reachable);
            }

            foreach (var res in reachableResults)
            {
                var item = res.Key;
                var locations = res.Value;

                var isReserved = false;
                foreach (var kvp in reservedLocations)
                {
                    if (kvp.Value.Equals(item))
                    {
                        var newReserved = new Dictionary<Location, Item>(reservedLocations);
                        newReserved.Remove(kvp.Key);

                        var newOutcome = outcome.WithItemInLocation(kvp.Value, kvp.Key);

                        if (newOutcome.IsCompletable())
                        {
                            Shuffle(compiled, newOutcome, newReserved);
                        }
                        else
                        {
                            CompileOutcome(0, compiled, new FailedOutcome());
                        }
                        isReserved = true;
                        break;
                    }
                }

                if (!isReserved)
                {
                    if (locations.Count == 0)
                    {
                        CompileOutcome(0, compiled, new FailedOutcome());
                        continue;
                    }
                    else
                    {
                        foreach (var location in locations)
                        {
                            if (reservedLocations.ContainsKey(location))
                            {
                                CompileOutcome(0, compiled, new FailedOutcome());
                                continue;
                            }
                            else
                            {
                                Shuffle(compiled, outcome.WithItemInLocation(item, location), reservedLocations);
                            };
                        }
                    }
                }
            }
        }

        public List<Item> FetchAllItems(List<Item> unplacedItems, SortedList<Location, Item> world)
        {
            var foundItems = new List<Item>(unplacedItems);
            bool found = true;
            var unobtainedLocations = new Dictionary<Location, Item>();

            foreach (var kvp in world)
            {
                if (kvp.Value != null)
                {
                    unobtainedLocations.Add(kvp.Key, kvp.Value);
                }
            }

            while (found && unobtainedLocations.Count != 0)
            {
                found = false;
                var newUnobtained = new Dictionary<Location, Item>();

                foreach (var loc in unobtainedLocations)
                {
                    if (loc.Key.CanBeReachedWith(foundItems))
                    {
                        found = true;
                        foundItems.Add(loc.Value);
                    }
                    else
                    {
                        newUnobtained.Add(loc.Key, loc.Value);
                    }
                }

                unobtainedLocations = newUnobtained;
            }

            return foundItems;
        }
    }
}
