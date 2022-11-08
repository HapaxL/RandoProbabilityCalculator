using System;
using System.Collections.Generic;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer.ResultCompiler
{
    //public class ResultWithParentsCompiler
    //{
    //    public static Type MyType = typeof(int);

        public class ResultWithParents
        {
            public int Count;
            public long Proportion;
            public Dictionary<string, long> Parents;

            public ResultWithParents(int count, long proportion, Dictionary<string, long> parents)
            {
                Count = count;
                Proportion = proportion;
                Parents = parents;
            }
        }
    //}
}
