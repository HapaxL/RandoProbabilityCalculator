using System;
using System.Collections.Generic;
using System.Text;
using HapaxTools;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class CompiledResultTree
    {
        public bool IsLeaf;
        public List<CompiledResultTree> Children;
        public CompiledResult Leaf;


    }
}
