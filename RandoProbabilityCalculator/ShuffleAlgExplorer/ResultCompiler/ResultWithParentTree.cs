using System;
using System.Collections.Generic;
using System.Text;
using HapaxTools;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer.ResultCompiler
{
    public class ResultWithParentTree
    {
        public bool IsLeaf;
        public List<ResultWithParentTree> Children;
        public ResultWithParents Leaf;


    }
}
