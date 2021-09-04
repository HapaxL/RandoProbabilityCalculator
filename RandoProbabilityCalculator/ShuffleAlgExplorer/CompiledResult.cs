using System;
using System.Collections.Generic;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class CompiledResult
    {
        public int Count;
        public long Proportion;
        public Dictionary<string, long> Parents;

        public CompiledResult(int count, long proportion, Dictionary<string, long> parents)
        {
            Count = count;
            Proportion = proportion;
            Parents = parents;
        }
    }
}
