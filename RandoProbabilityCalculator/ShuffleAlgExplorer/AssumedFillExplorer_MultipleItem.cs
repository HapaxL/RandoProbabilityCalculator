//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using HapaxTools;

//namespace RandoProbabilityCalculator.ShuffleAlgExplorer
//{
//    public class AssumedFillExplorer_MultipleItem : Algorithm
//    {
//        int ocCount = 0;

//        public override Dictionary<string, int> Shuffle(Outcome outcome)
//        {
//            Console.WriteLine("starting singleitem");
//            var compiled = new Dictionary<string, int>();
//            Shuffle(compiled, outcome, new List<Item>());
//            Console.WriteLine("ending singleitem");
//            return compiled;
//        }

//        public void Shuffle(Dictionary<string, int> compiled, Outcome outcome, IEnumerable<Item> PriorityQueue)
//        {
//            // Console.WriteLine($"{outcome.UnplacedItems.Count} unplaced items");

//            if (outcome.UnplacedItems.Count == 0 || outcome.EmptyLocations.Count == 0)
//            {
//                ocCount++;
//                if (ocCount % 1000 == 0) Console.WriteLine(ocCount);
//                CompileOutcome(ocCount, compiled, outcome);
//                //if (ocCount == 682922)
//                //{
//                //    Console.WriteLine("problematic");
//                //}
//                return;
//            }

//            var reachableResults = new Dictionary<Item, List<Location>>();

//            var newOutcome = outcome;

//            IEnumerable<Item> toPlace;

//            if (PriorityQueue.Count() == 0)
//            {
//                toPlace = newOutcome.UnplacedItems;
//            }
//            else
//            {
//                toPlace = PriorityQueue;
//            }
            
//            foreach (var item in toPlace.Distinct())
//            {
//                var unplaced = new List<Item>(toPlace);
//                unplaced.Remove(item);
//                var allItems = FetchAllItems(unplaced, newOutcome.World);
//                var reachable = newOutcome.EmptyLocations.Where(l => l.CanBeReachedWith(allItems)).ToList();
//                reachableResults.Add(item, reachable);
//            }

//            var resultSet = reachableResults.ToHashSet();
//            var resultSubsets = resultSet.Powerset()
//                .Where(s => s.Count > 0)
//                .OrderBy(s => s.Count)
//                .Select(s => new KeyValuePair<IEnumerable<Item>, ISet<Location>>(
//                    s.Select(kvp => kvp.Key),
//                    s.SelectMany(kvp => kvp.Value).ToHashSet()));
            
//            var first = resultSubsets.FirstOrDefault(subset => subset.Key.Count() == subset.Value.Count);
//            if (!first.Equals(default(KeyValuePair<IEnumerable<Item>, ISet<Location>>)))
//            {
//                Shuffle(compiled, outcome, PriorityQueue.Concat(first.Key));
//            }

//            foreach (var result in reachableResults)
//            {
//                if (result.Value.Count == 0)
//                {
//                    CompileOutcome(0, compiled, new FailedOutcome());
//                    continue;
//                }
//                else if (result.Value.Count == 1)
//                {
//                    Shuffle(compiled, newOutcome);
//                }
//                else
//                {
//                    foreach (var location in result.Value)
//                    {
//                        Shuffle(compiled, newOutcome.WithItemInLocation(result.Key, location));
//                    }
//                }
//            }
//        }

//        public List<Item> FetchAllItems(List<Item> unplacedItems, SortedList<Location, Item> world)
//        {
//            var foundItems = new List<Item>(unplacedItems);
//            bool found = true;
//            var unobtainedLocations = new Dictionary<Location, Item>();

//            foreach (var kvp in world)
//            {
//                if (kvp.Value != null)
//                {
//                    unobtainedLocations.Add(kvp.Key, kvp.Value);
//                }
//            }

//            while (found && unobtainedLocations.Count != 0)
//            {
//                found = false;
//                var newUnobtained = new Dictionary<Location, Item>();

//                foreach (var loc in unobtainedLocations)
//                {
//                    if (loc.Key.CanBeReachedWith(foundItems))
//                    {
//                        found = true;
//                        foundItems.Add(loc.Value);
//                    }
//                    else
//                    {
//                        newUnobtained.Add(loc.Key, loc.Value);
//                    }
//                }

//                unobtainedLocations = newUnobtained;
//            }

//            return foundItems;
//        }
//    }
//}
