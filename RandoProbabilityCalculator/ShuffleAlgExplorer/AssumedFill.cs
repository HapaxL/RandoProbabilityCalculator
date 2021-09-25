using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class AssumedFill : Algorithm
    {
        int ocCount = 0;

        public override Dictionary<string, CompiledResult> Shuffle(Outcome outcome)
        {
            Console.WriteLine("starting assumed new");
            var perms = GetPermutations(outcome.UnplacedItems);

            var compileds = new List<Dictionary<string, CompiledResult>>();
            foreach (var perm in perms)
            {
                var compiled = SubShuffle(outcome, perm, outcome.GetWorldString(0));
                compileds.Add(compiled);
            }
            Console.WriteLine("ending assumed new");
            return CompileOutcomes(compileds);
        }

        public Dictionary<string, CompiledResult> SubShuffle(Outcome outcome, List<Item> permutation, string parent)
        {
            // Console.WriteLine($"{outcome.UnplacedItems.Count} unplaced items");

            if (permutation.Count == 0 || outcome.EmptyLocations.Count == 0)
            {
                ocCount++;
                if (ocCount % 1000 == 0) Console.WriteLine(ocCount);
                // CompileOutcome(ocCount, compiled, outcome);
                //if (ocCount == 682922)
                //{
                //    Console.WriteLine("problematic");
                //}
                return CompileSingleOutcome(ocCount, outcome, new Dictionary<string, long> { { parent, 1 } });
            }

            var compileds = new List<Dictionary<string, CompiledResult>>();

            var item = permutation[0];
            var unplaced = permutation.Skip(1).ToList();
            var allItems = FetchAllItems(unplaced, outcome.World);
            var reachableEmptyLocs = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems));

            if (reachableEmptyLocs.Count() == 0)
            {
                var compiled = CompileSingleOutcome(ocCount, Outcome.Failed, new Dictionary<string, long> { { outcome.GetWorldString(0), 1 } });
                return compiled;
            }

            foreach (var location in reachableEmptyLocs)
            {
                var compiled = SubShuffle(outcome.WithItemInLocation(item, location), new List<Item>(unplaced), outcome.GetWorldString(ocCount));
                compileds.Add(compiled);
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
