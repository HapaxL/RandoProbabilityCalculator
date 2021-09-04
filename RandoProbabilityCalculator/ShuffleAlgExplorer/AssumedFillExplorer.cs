using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class AssumedFillExplorer : AlgorithmExplorer
    {
        int ocCount = 0;

        public override Dictionary<string, KeyValuePair<int, long>> Shuffle(Outcome outcome)
        {
            Console.WriteLine("starting assumed");
            var compiled = SubShuffle(outcome);
            Console.WriteLine("ending assumed");
            return compiled;
        }

        public Dictionary<string, KeyValuePair<int, long>> SubShuffle(Outcome outcome)
        {
            // Console.WriteLine($"{outcome.UnplacedItems.Count} unplaced items");

            if (outcome.UnplacedItems.Count == 0 || outcome.EmptyLocations.Count == 0)
            {
                ocCount++;
                if (ocCount % 1000 == 0) Console.WriteLine(ocCount);
                // CompileOutcome(ocCount, compiled, outcome);
                //if (ocCount == 682922)
                //{
                //    Console.WriteLine("problematic");
                //}
                return CompileSingleOutcome(ocCount, outcome);
            }

            var compileds = new List<Dictionary<string, KeyValuePair<int, long>>>();

            foreach (var item in outcome.UnplacedItems)
            {
                var unplaced = new List<Item>(outcome.UnplacedItems);
                unplaced.Remove(item);
                var allItems = FetchAllItems(unplaced, outcome.World);
                var reachableEmptyLocs = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems));

                if (reachableEmptyLocs.Count() == 0)
                {
                    var compiled = CompileSingleOutcome(ocCount, Outcome.Failed);
                    compileds.Add(compiled);
                    continue;
                }

                foreach (var location in reachableEmptyLocs)
                {
                    var compiled = SubShuffle(outcome.WithItemInLocation(item, location));
                    compileds.Add(compiled);
                }
            }

            return CompileOutcomes(compileds);
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
