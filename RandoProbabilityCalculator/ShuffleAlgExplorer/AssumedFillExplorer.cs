using System;
using System.Collections.Generic;
using System.Linq;
using RandoProbabilityCalculator.ShuffleAlgExplorer.ResultCompiler;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class AssumedFillExplorer : Algorithm
    {
        public bool IgnoreDuplicates { get; private set; }
        int ocCount = 0;

        public AssumedFillExplorer(bool ignoreDuplicates)
        {
            IgnoreDuplicates = ignoreDuplicates;
        }

        public override Dictionary<string, ResultWithParents> Shuffle(Outcome outcome)
        {
            Dictionary<Item, ResultWithParents> compiled;
            Console.WriteLine("starting assumedexplorer");
            compiled = SubShuffle(outcome, 0);
            Console.WriteLine("ending assumedexplorer");
            return compiled;
        }

        public Dictionary<string, ResultWithParents> SubShuffle(Outcome outcome, int depth)
        {
            // Console.WriteLine($"{outcome.UnplacedItems.Count} unplaced items");

            if (outcome.UnplacedItems.Count == 0 || outcome.EmptyLocations.Count == 0)
            {
                ocCount++;
                if (ocCount % 10000 == 0) Console.WriteLine(ocCount);
                // CompileOutcome(ocCount, compiled, outcome);
                //if (ocCount == 682922)
                //{
                //    Console.WriteLine("problematic");
                //}
                // Console.WriteLine($"{outcome.GetWorldString(depth)} end");
                return CompileSingleOutcome(outcome);
            }

            var outcomes = new List<Dictionary<string, ResultWithParents>>();

            IEnumerable<Item> toBePlaced;

            if (IgnoreDuplicates)
            {
                toBePlaced = outcome.UnplacedItems.Distinct();
            }
            else
            {
                toBePlaced = outcome.UnplacedItems;
            }

            foreach (var item in toBePlaced)
            {
                var unplaced = new List<Item>(outcome.UnplacedItems);
                unplaced.Remove(item);
                var allItems = FetchAllItems(unplaced, outcome.World);
                var reachableEmptyLocs = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems));
                var locOutcomes = new List<Dictionary<string, ResultWithParents>>();

                if (reachableEmptyLocs.Count() == 0)
                {
                    // Console.WriteLine($"{outcome.GetWorldString(depth)} <- {item} (failed)");
                    var oc = CompileSingleOutcome(Outcome.Failed);
                    outcomes.Add(oc);
                    continue;
                }

                // Console.WriteLine($"{outcome.GetWorldString(depth)} <- {item}");
                foreach (var location in reachableEmptyLocs)
                {
                    var oc = SubShuffle(outcome.WithItemInLocation(item, location), depth+1);
                    locOutcomes.Add(oc);
                }

                var locCompiled = CompileOutcomes(locOutcomes);
                outcomes.Add(locCompiled);
            }

            return CompileOutcomes(outcomes);
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
