using System;
using System.Collections.Generic;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public abstract class Algorithm
    {
        public abstract Dictionary<string, int> Shuffle(Outcome world);

        public void CompileOutcome(int count, Dictionary<string, int> compiled, Outcome outcome)
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
    }
}
