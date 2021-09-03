using System;
using System.Collections.Generic;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public abstract class Algorithm
    {
        public abstract Dictionary<string, long> Shuffle(Outcome world);

        public void CompileOutcome(int count, Dictionary<string, long> compiled, Outcome outcome)
        {
            var s = outcome.GetWorldString(count);

            if (compiled.ContainsKey(s))
            {
                compiled[s]++;
            }
            else
            {
                compiled.Add(s, 1);
            }
        }

        public Dictionary<string, long> CompileSingleOutcome(int count, Outcome outcome)
        {
            var s = outcome.GetWorldString(count);
            return new Dictionary<string, long>() { { s, 1 } };
        }
    }
}
