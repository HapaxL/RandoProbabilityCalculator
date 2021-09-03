using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public abstract class Algorithm
    {
        public abstract Dictionary<string, long> Shuffle(Outcome world);

        public Dictionary<string, long> CompileOutcomes(IEnumerable<Dictionary<string, long>> compileds)
        {
            if (compileds.Count() == 0)
            {
                return CompileSingleOutcome(0, new FailedOutcome());
            }

            var totals = compileds.ToDictionary(c => c, c => c.Values.Sum());
            var lcm = HapaxTools.Math.LeastCommonMultiple(totals.Values.ToList());

            var allCompiled = new Dictionary<string, long>();

            foreach (var compiled in compileds)
            {
                var total = totals[compiled];

                foreach (var kvp in compiled)
                {
                    var oc = kvp.Key;
                    var probaRate = kvp.Value * (lcm / total);

                    if (allCompiled.ContainsKey(oc))
                    {
                        allCompiled[oc] += probaRate;
                    }
                    else
                    {
                        allCompiled.Add(oc, probaRate);
                    }
                }
            }

            return allCompiled;
        }

        public Dictionary<string, long> CompileSingleOutcome(int count, Outcome outcome)
        {
            var s = outcome.GetWorldString(count);
            return new Dictionary<string, long>() { { s, 1 } };
        }
    }
}
