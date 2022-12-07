using System;
using System.Collections.Generic;
using System.Linq;
using RandoProbabilityCalculator.ShuffleAlgExplorer.ResultCompiler;
using HapaxTools;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class AssumedFill : Algorithm
    {
        public bool IgnoreJunk { get; private set; }
        int ocCount = 0;

        public AssumedFill(bool ignoreJunk)
        {
            IgnoreJunk = ignoreJunk;
        }

        public override Dictionary<string, ResultWithParents> Shuffle(Outcome outcome)
        {
            var compileds = new List<Dictionary<string, ResultWithParents>>();
            List<List<Item>> perms;

            var time = new System.Diagnostics.Stopwatch();
            time.Start();

            if (IgnoreJunk)
            {
                Console.WriteLine("starting assumed (ignoring junk)");
                var useful = GetUsefulItems(outcome.UnplacedItems, outcome.EmptyLocations);
                var junk = outcome.UnplacedItems.SubtractRange(useful);
                var usefulPerms = GetPermutations(useful.ToList());
                var junkPerms = GetPermutations(junk.ToList());
                perms = new List<List<Item>>();

                foreach (var u in usefulPerms)
                {
                    foreach (var j in junkPerms)
                    {
                        perms.Add(u.Concat(j).ToList());
                    }
                }
            }
            else
            {
                Console.WriteLine("starting assumed");
                perms = GetPermutations(outcome.UnplacedItems);
            }

            var t1 = time.ElapsedMilliseconds;
            time.Restart();

            foreach (var perm in perms)
            {
                //foreach (var i in perm)
                //{
                //    Console.Write(i);
                //}
                // Console.WriteLine();
                // var compiled = SubShuffle(outcome, perm, outcome.GetWorldString(0));

                var compiled = SubShuffle(outcome, perm);
                compileds.Add(compiled);
            }

            var t2 = time.ElapsedMilliseconds;
            time.Stop();

            Console.WriteLine("ending assumed");
            Console.WriteLine($"time elapsed: {t1}ms generating perms, {t2}ms shuffling");
            return CompileOutcomes(compileds);
        }

        public Dictionary<List<Item>, Dictionary<string, ResultWithParents>> ShufflePermsSeparately(Outcome outcome)
        {
            Console.WriteLine("starting assumed (shuffling perms separately)");
            var perms = GetPermutations(outcome.UnplacedItems);
            var compileds = new Dictionary<List<Item>, Dictionary<string, ResultWithParents>>();
            foreach (var perm in perms)
            {
                // var compiled = SubShuffle(outcome, perm, outcome.GetWorldString(0));
                var compiled = SubShuffle(outcome, perm);
                compileds.Add(perm, compiled);
            }
            Console.WriteLine("ending assumed");
            return compileds;
        }

        // public Dictionary<string, ResultWithParents> SubShuffle(Outcome outcome, List<Item> permutation, string parent)
        public Dictionary<string, ResultWithParents> SubShuffle(Outcome outcome, List<Item> permutation)
        {
            // Console.WriteLine($"{outcome.UnplacedItems.Count} unplaced items");

            if (permutation.Count == 0 || outcome.EmptyLocations.Count == 0)
            {
                ocCount++;
                if (ocCount % 10000 == 0) Console.WriteLine(ocCount);
                // CompileOutcome(ocCount, compiled, outcome);
                //if (ocCount == 682922)
                //{
                //    Console.WriteLine("problematic");
                //}
                // return CompileSingleOutcome(outcome, new Dictionary<string, long> { { parent, 1 } });
                return CompileSingleOutcome(outcome);
            }

            var compileds = new List<Dictionary<string, ResultWithParents>>();

            var unplaced = new List<Item>(permutation);
            var item = unplaced[0];
            unplaced.Remove(item);
            var allItems = FetchAllItems(unplaced, outcome.World);
            var reachableEmptyLocs = outcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems));

            if (reachableEmptyLocs.Count() == 0)
            {
                // var compiled = CompileSingleOutcome(Outcome.Failed, new Dictionary<string, long> { { outcome.GetWorldString(0), 1 } });
                var compiled = CompileSingleOutcome(Outcome.Failed);
                return compiled;
            }

            foreach (var location in reachableEmptyLocs)
            {
                // var compiled = SubShuffle(outcome.WithItemInLocation(item, location), new List<Item>(unplaced), outcome.GetWorldString(ocCount));
                var compiled = SubShuffle(outcome.WithItemInLocation(item, location), unplaced);
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
