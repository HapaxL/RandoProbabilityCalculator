﻿using System;
using System.Collections.Generic;
using System.Linq;
using RandoProbabilityCalculator.ShuffleAlgExplorer.ResultCompiler;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class AssumedFillExplorer_SingleItem : Algorithm
    {
        int ocCount = 0;

        public override Dictionary<string, ResultWithParents> Shuffle(Outcome outcome)
        {
            Console.WriteLine("starting singleitem");
            var compiled = SubShuffle(outcome);
            Console.WriteLine("ending singleitem");
            return compiled;
        }

        public Dictionary<string, ResultWithParents> SubShuffle(Outcome outcome)
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
                return CompileSingleOutcome(outcome);
            }

            var compileds = new List<Dictionary<string, ResultWithParents>>();

            var reachableResults = new Dictionary<Item, List<Location>>();

            var newOutcome = outcome;

            foreach (var item in outcome.UnplacedItems.Distinct())
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
                    var compiled = CompileSingleOutcome(Outcome.Failed);
                    compileds.Add(compiled);
                    continue;
                }
                else if (result.Value.Count == 1)
                {
                    var compiled = SubShuffle(newOutcome);
                    compileds.Add(compiled);
                }
                else
                {
                    foreach (var location in result.Value)
                    {
                        var compiled = SubShuffle(newOutcome.WithItemInLocation(result.Key, location));
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
