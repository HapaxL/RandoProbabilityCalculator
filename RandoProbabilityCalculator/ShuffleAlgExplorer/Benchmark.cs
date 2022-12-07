using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class Benchmark
    {
        Dictionary<string, Algorithm> Algorithms;
        Dictionary<string, Dictionary<Item, ResultCompiler.ResultWithParents>> Results;
        Dictionary<string, long> Times;

        public Benchmark()
        {
            Algorithms = new Dictionary<string, Algorithm>();
        }

        public void Add(string name, Algorithm alg)
        {
            Algorithms.Add(name, alg);
        }

        public void Shuffle(Outcome oc)
        {
            Results = new Dictionary<Item, Dictionary<Item, ResultCompiler.ResultWithParents>>();
            Times = new Dictionary<Item, long>();

            var watch = new Stopwatch();
            foreach (var alg in Algorithms)
            {
                watch.Restart();
                var res = alg.Value.Shuffle(oc);
                var time = watch.ElapsedMilliseconds;
                Results.Add(alg.Key, res);
                Times.Add(alg.Key, time);
            }
        }

        public void PrintResults()
        {
            foreach (var alg in Algorithms)
            {
                Algorithm.PrintResults(alg.Key, Results[alg.Key], false);
            }
        }

        public void PrintStats()
        {
            foreach (var alg in Algorithms)
            {
                Algorithm.PrintStats($"{alg.Key}: {Times[alg.Key]}ms", Results[alg.Key]);
            }
        }

        public void Run(Outcome oc)
        {
            Shuffle(oc);
            Console.WriteLine();
            PrintResults();
            Console.WriteLine();
            PrintStats();
        }
    }
}
