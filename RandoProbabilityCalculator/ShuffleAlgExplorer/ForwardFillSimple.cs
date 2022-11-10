using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandoProbabilityCalculator.ShuffleAlgExplorer.ResultCompiler;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    // simple (no dangerous actions, assumes all placed items can be obtained) version of the forward fill alg
    public class ForwardFillSimple : Algorithm
    {
        int ocCount = 0;
        // mustOpenLocations: the item being placed must open new locations that weren't available before, unless all locations have been opened.
        public bool MustOpenLocations { get; private set; }
        // mustLeaveOpenLocations: at least one open location must remain after the item has been placed.
        public bool MustLeaveOpenLocations { get; private set; }
        // dynamicItemPicking (previously called Explorer): if true, will branch out on every step of the filling 
        // algorithm for every remaining item. if false, will generate all possible permutations of items beforehand
        // and will run the algorithm once for each one statically, placing items in order of the given permutation.
        public bool DynamicItemPicking { get; private set; }

        public ForwardFillSimple(bool mustOpenLocations, bool mustLeaveOpenLocations)
        {
            MustOpenLocations = mustOpenLocations;
            MustLeaveOpenLocations = mustLeaveOpenLocations;
            DynamicItemPicking = true; // TODO
        }

        public override Dictionary<string, ResultWithParents> Shuffle(Outcome outcome)
        {
            return Shuffle_temp(outcome);
        }

        public Dictionary<string, ResultWithParents> Shuffle_temp(Outcome outcome)
        {
            var sb = new StringBuilder("starting forward");
            sb.Append(DynamicItemPicking ? " dynamic" : " static");
            if (MustOpenLocations)
            {
                sb.Append(" (mustOpenLocations)");
            }
            else if (MustLeaveOpenLocations)
            {
                sb.Append(" (mustLeaveOpenLocations)");
            }
            else
            {
                sb.Append(" (no restrictions)");
            }
            Console.WriteLine(sb.ToString());
            var compiled = SubShuffle(outcome, GetPlaceableItemsFunction());
            Console.WriteLine($"ending forward");
            return compiled;
        }

        public Dictionary<string, ResultWithParents> SubShuffle(Outcome outcome, Func<Outcome, IEnumerable<Item>> getPlaceableItemsFunc)
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

            var compileds = new List<Dictionary<string, ResultWithParents>>();

            var placeableItems = getPlaceableItemsFunc(outcome);
            foreach (var item in placeableItems)
            {
                var allPlaced = new List<Item>(outcome.PlacedItems);
                //allPlaced.Add(item);

                var reachableEmptyLocs = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(allPlaced));

                if (reachableEmptyLocs.Count() == 0)
                {
                    var compiled = CompileSingleOutcome(ocCount, Outcome.Failed);
                    compileds.Add(compiled);
                    continue;
                }

                foreach (var location in reachableEmptyLocs)
                {
                    var compiled = SubShuffle(outcome.WithItemInLocation(item, location), getPlaceableItemsFunc);
                    compileds.Add(compiled);
                }
            }

            return CompileOutcomes(compileds);
        }

        public Func<Outcome, IEnumerable<Item>> GetPlaceableItemsFunction()
        {
            Func<Outcome, IEnumerable<Item>> fetchPlaceableItems;

            if (MustOpenLocations)
            {
                fetchPlaceableItems = outcome =>
                {
                    return outcome.UnplacedItems.Where(i =>
                    {
                        var reachableEmptyLocsBefore = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(outcome.PlacedItems));
                        var allPlaced = new List<Item>(outcome.PlacedItems);
                        allPlaced.Add(i);
                        var reachableEmptyLocsAfter = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(allPlaced));
                        var beforeCount = reachableEmptyLocsBefore.Count();
                        var afterCount = reachableEmptyLocsAfter.Count();
                        return afterCount + outcome.PlacedItems.Count == outcome.World.Count || afterCount > beforeCount;
                    });
                };
            }
            else if (MustLeaveOpenLocations)
            {
                fetchPlaceableItems = outcome =>
                {
                    return outcome.UnplacedItems.Where(i =>
                    {
                        var allPlaced = new List<Item>(outcome.PlacedItems);
                        allPlaced.Add(i);
                        var reachableEmptyLocs = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(allPlaced));
                        var count = reachableEmptyLocs.Count();
                        return count + outcome.PlacedItems.Count == outcome.World.Count || count > 1;
                    });
                };
            }
            else
            {
                fetchPlaceableItems = outcome => outcome.UnplacedItems;
            }

            return fetchPlaceableItems;
        }


        // forward fill:
        // - empty world, all items unobtained
        // - find all items that can be placed, open a location (if 1), and leave open a location (if 2)
        // - place it in a random available space
        // - 
    }
}
