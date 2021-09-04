using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public abstract class AlgorithmExplorer
    {
        public abstract Dictionary<string, KeyValuePair<int, long>> Shuffle(Outcome world);

        public static Dictionary<string, KeyValuePair<int, long>> CompileOutcomes(IEnumerable<Dictionary<string, KeyValuePair<int, long>>> compileds)
        {
            if (compileds.Count() == 0)
            {
                return CompileSingleOutcome(0, Outcome.Failed);
            }

            var totals = compileds.ToDictionary(c => c, c => c.Values.Select(kvp => kvp.Value).Sum());
            var lcm = HapaxTools.Math.LeastCommonMultiple(totals.Values.ToList());

            var allCompiled = new Dictionary<string, KeyValuePair<int, long>>();

            foreach (var compiled in compileds)
            {
                var total = totals[compiled];

                foreach (var kvp in compiled)
                {
                    var oc = kvp.Key;
                    var count = kvp.Value.Key;
                    var probaRate = kvp.Value.Value * (lcm / total);

                    if (allCompiled.ContainsKey(oc))
                    {
                        allCompiled[oc] = new KeyValuePair<int, long>(allCompiled[oc].Key + count, allCompiled[oc].Value + probaRate);
                    }
                    else
                    {
                        allCompiled.Add(oc, new KeyValuePair<int, long>(count, probaRate));
                    }
                }
            }

            return allCompiled;
        }

        public static Dictionary<string, KeyValuePair<int, long>> CompileSingleOutcome(int count, Outcome outcome)
        {
            var s = outcome.GetWorldString(count);
            return new Dictionary<string, KeyValuePair<int, long>>() { { s, new KeyValuePair<int, long>(1, 1) } };
        }
    }
}
