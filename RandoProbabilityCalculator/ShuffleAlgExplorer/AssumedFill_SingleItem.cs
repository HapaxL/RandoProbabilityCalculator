using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class AssumedFill_SingleItem : Algorithm
    {
        int ocCount = 0;

        public override Dictionary<string, CompiledResult> Shuffle(Outcome outcome)
        {
            Console.WriteLine("starting singleitem");
            var perms = GetPermutations(outcome.UnplacedItems);

            var compileds = new List<Dictionary<string, CompiledResult>>();
            foreach (var perm in perms)
            {
                var compiled = SubShuffle(outcome, perm, outcome.GetWorldString(0));
                compileds.Add(compiled);
            }
            Console.WriteLine("ending singleitem");
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
            var newOutcome = outcome;
            var unplaced = new List<Item>(permutation);
            var found = true;
            while(found)
            {
                var newUnplaced = new List<Item>(unplaced);
                found = false;
                foreach (var item in newUnplaced)
                {
                    var allItems = FetchAllItems(newUnplaced, newOutcome.World);
                    var reachable = newOutcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems)).ToList();

                    if (reachable.Count == 0)
                    {
                        var compiled = CompileSingleOutcome(ocCount, Outcome.Failed, new Dictionary<string, long> { { outcome.GetWorldString(ocCount), 1 } });
                        return compiled;
                    }
                    else if (reachable.Count == 1)
                    {
                        found = true;
                        newOutcome = newOutcome.WithItemInLocation(item, reachable[0]);
                        unplaced.Remove(item);
                        break;
                    }
                }
            }

            if (unplaced.Count == 0)
            {
                if (newOutcome.IsCompletable())
                {
                    var compiled = SubShuffle(newOutcome, unplaced, newOutcome.GetWorldString(ocCount));
                    compileds.Add(compiled);
                }
                else
                {
                    var compiled = CompileSingleOutcome(0, Outcome.Failed, new Dictionary<string, long> { { outcome.GetWorldString(ocCount), 1 } });
                    return compiled;
                }
            }
            else
            {
                var item1 = unplaced[0];
                var newUnplaced1 = unplaced.Skip(1).ToList();
                var allItems1 = FetchAllItems(newUnplaced1, newOutcome.World);
                var reachableEmptyLocs = newOutcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems1));

                if (reachableEmptyLocs.Count() == 0)
                {
                    var compiled = CompileSingleOutcome(0, Outcome.Failed, new Dictionary<string, long> { { outcome.GetWorldString(ocCount), 1 } });
                    return compiled;
                }
                else
                {
                    foreach (var location in reachableEmptyLocs)
                    {
                        var compiled = SubShuffle(newOutcome.WithItemInLocation(item1, location), new List<Item>(newUnplaced1), newOutcome.GetWorldString(ocCount));
                        compileds.Add(compiled);
                    }
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
