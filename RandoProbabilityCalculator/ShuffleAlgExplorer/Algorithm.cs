using System;
using System.Collections.Generic;
using System.Linq;
using RandoProbabilityCalculator.ShuffleAlgExplorer.ResultCompiler;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public abstract class Algorithm
    {
        public abstract Dictionary<string, ResultWithParents> Shuffle(Outcome world);

        public static List<List<Item>> GetPermutations(List<Item> items)
        {
            if (items.Count == 1)
            {
                return new List<List<Item>>
                {
                    items,
                };
            }

            var perms = new List<List<Item>>();

            foreach (var l in items)
            {
                var subList = new List<Item>(items);
                subList.Remove(l);
                var subPerms = GetPermutations(subList);
                foreach (var p in subPerms)
                {
                    p.Add(l);
                    perms.Add(p);
                }
            }

            return perms;
        }
        public static Dictionary<string, ResultWithParents> CompileOutcomes(IEnumerable<Dictionary<string, ResultWithParents>> compileds)
        {
            if (compileds.Count() == 0)
            {
                return CompileSingleOutcome(0, Outcome.Failed, new Dictionary<string, long> { { "???", 1 } });
            }

            var totals = compileds.ToDictionary(c => c, c => c.Values.Select(co => co.Proportion).Sum());
            var lcm = HapaxTools.Math.LeastCommonMultiple(totals.Values);

            var allCompiled = new Dictionary<string, ResultWithParents>();

            foreach (var compiled in compileds)
            {
                var total = totals[compiled];

                foreach (var kvp in compiled)
                {
                    var oc = kvp.Key;
                    var count = kvp.Value.Count;
                    var probaRate = kvp.Value.Proportion * (lcm / total);

                    Dictionary<string, long> newParents;

                    if (allCompiled.ContainsKey(oc))
                    {
                        newParents = new Dictionary<string, long>(allCompiled[oc].Parents);
                    }
                    else
                    {
                        newParents = new Dictionary<string, long>();
                    }

                    foreach (var parent in kvp.Value.Parents)
                    {
                        var parentProbaRate = parent.Value * (lcm / total);

                        if (newParents.ContainsKey(parent.Key))
                        {
                            newParents[parent.Key] += parentProbaRate;
                        }
                        else
                        {
                            newParents.Add(parent.Key, parentProbaRate);
                        }
                    }

                    if (allCompiled.ContainsKey(oc))
                    {
                        allCompiled[oc] = new ResultWithParents(allCompiled[oc].Count + count, allCompiled[oc].Proportion + probaRate, newParents);
                    }
                    else
                    {
                        allCompiled.Add(oc, new ResultWithParents(count, probaRate, newParents));
                    }
                }
            }

            return allCompiled;
        }

        public static Dictionary<string, ResultWithParents> CompileSingleOutcome(int count, Outcome outcome, Dictionary<Item, long> parents)
        {
            var s = outcome.GetWorldString(count);
            return new Dictionary<string, ResultWithParents>() { { s, new ResultWithParents(1, 1, parents) } };
        }

        public static Dictionary<string, ResultWithParents> CompileSingleOutcome(int count, Outcome outcome)
        {
            return CompileSingleOutcome(count, outcome, new Dictionary<Item, long>());
        }

        public static void PrintResults(string name, Dictionary<string, ResultWithParents> compiled, bool printParents)
        {
            Console.WriteLine();
            Console.WriteLine(name);

            var failedOutcomeString = Outcome.Failed.GetWorldString(0);
            var total = compiled.Values.Select(kvp => kvp.Proportion).Sum();
            var totalSuccesses = compiled.ContainsKey(failedOutcomeString) ? (total - compiled[failedOutcomeString].Proportion) : total;

            foreach (var c in compiled)
            {
                if (c.Key == failedOutcomeString)
                {
                    Console.WriteLine($"{c.Key}: ({c.Value.Count}) {c.Value.Proportion} ({100.0 * c.Value.Proportion / total}% of total)");
                    if (printParents)
                    {
                        foreach (var parent in c.Value.Parents)
                        {
                            Console.WriteLine($"    {parent.Key}: {parent.Value} ({100.0 * parent.Value / total}% of total)");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{c.Key}: ({c.Value.Count}) {c.Value.Proportion} ({100.0 * c.Value.Proportion / total}% of total, {100.0 * c.Value.Proportion / totalSuccesses}% of successes)");
                    if (printParents)
                    {
                        foreach (var parent in c.Value.Parents)
                        {
                            Console.WriteLine($"    {parent.Key}: {parent.Value} ({100.0 * parent.Value / total}% of total, {100.0 * parent.Value / totalSuccesses}% of successes)");
                        }
                    }
                }
            }
        }
        
        public static void PrintStats(string name, Dictionary<string, ResultWithParents> compiled)
        {
            Console.WriteLine();
            Console.WriteLine(name);

            var failedOutcomeString = Outcome.Failed.GetWorldString(0);
            var total = compiled.Values.Select(kvp => kvp.Proportion).Sum();
            var totalSuccesses = compiled.ContainsKey(failedOutcomeString) ? (total - compiled[failedOutcomeString].Proportion) : total;
            var successfulEntryCount = compiled.ContainsKey(failedOutcomeString) ? compiled.Count - 1 : compiled.Count;
            var proportionValues = new List<long>();
            double sum = 0;
            var leastLikelyOutcome = default(KeyValuePair<Item, ResultWithParents>);
            var mostLikelyOutcome = default(KeyValuePair<Item, ResultWithParents>);

            foreach (var c in compiled)
            {
                if (c.Key == failedOutcomeString)
                {
                    continue;
                }

                proportionValues.Add(c.Value.Proportion);
                sum += c.Value.Proportion;
                if (leastLikelyOutcome.Value == null || leastLikelyOutcome.Value.Proportion > c.Value.Proportion)
                    leastLikelyOutcome = c;
                if (mostLikelyOutcome.Value == null || mostLikelyOutcome.Value.Proportion < c.Value.Proportion)
                    mostLikelyOutcome = c;
            }

            var span = mostLikelyOutcome.Value.Proportion - leastLikelyOutcome.Value.Proportion;
            double arithmeticMean = sum / proportionValues.Count;
            double median;
            proportionValues.Sort();
            if (successfulEntryCount % 2 == 0)
            {
                var midpoint = successfulEntryCount / 2;
                median = (proportionValues[midpoint - 1] + proportionValues[midpoint]) / 2;
            }
            else
            {
                median = proportionValues[(successfulEntryCount - 1) / 2];
            }

            double meanAbsoluteDeviation = proportionValues.Sum(p => Math.Abs(p - arithmeticMean)) / successfulEntryCount;
            double medianAbsoluteDeviation = proportionValues.Sum(p => Math.Abs(p - median)) / successfulEntryCount;
            
            Console.WriteLine($"Smallest probability: {leastLikelyOutcome.Key} ({leastLikelyOutcome.Value.Proportion}, {100.0 * leastLikelyOutcome.Value.Proportion / totalSuccesses}%)");
            Console.WriteLine($"Biggest probability: {mostLikelyOutcome.Key} ({mostLikelyOutcome.Value.Proportion}, {100.0 * mostLikelyOutcome.Value.Proportion / totalSuccesses}%)");
            Console.WriteLine($"Probability span: {100.0 * span / totalSuccesses}%");
            Console.WriteLine($"Arithmetic mean: {100.0 * arithmeticMean / totalSuccesses}%, mean absolute deviation: {100.0 * meanAbsoluteDeviation / totalSuccesses}%");
            Console.WriteLine($"Median: {100.0 * median / totalSuccesses}%, median absolute deviation: {100.0 * medianAbsoluteDeviation / totalSuccesses}%");
            Console.WriteLine("(all percentages relative to total successes)");
        }
    }
}
