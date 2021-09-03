using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class AssumedFill_SingleItem : Algorithm
    {
        int ocCount = 0;

        public override Dictionary<string, int> Shuffle(Outcome outcome)
        {
            Console.WriteLine("starting singleitem");
            var compiled = new Dictionary<string, int>();
            Shuffle(compiled, outcome);
            Console.WriteLine("ending singleitem");
            return compiled;
        }

        public void Shuffle(Dictionary<string, int> compiled, Outcome outcome)
        {
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

            var newOutcome = outcome;

            foreach (var item in newOutcome.UnplacedItems.Distinct())
            {
                var unplaced = new List<Item>(newOutcome.UnplacedItems);
                unplaced.Remove(item);
                var allItems = FetchAllItems(unplaced, newOutcome.World);
                var reachable = newOutcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems)).ToList();

                if (reachable.Count == 1)
                {
                    newOutcome = newOutcome.WithItemInLocation(item, reachable[0]);
                    foreach (var kvp in reachableResults)
                    {
                        kvp.Value.Remove(reachable[0]);
                    }
                }

                reachableResults.Add(item, reachable);
            }

            foreach (var result in reachableResults)
            {
                if (result.Value.Count == 0)
                {
                    CompileOutcome(0, compiled, new FailedOutcome());
                    continue;
                }
                else if (result.Value.Count == 1)
                {
                    Shuffle(compiled, newOutcome);
                }
                else
                {
                    foreach (var location in result.Value)
                    {
                        Shuffle(compiled, newOutcome.WithItemInLocation(result.Key, location));
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
