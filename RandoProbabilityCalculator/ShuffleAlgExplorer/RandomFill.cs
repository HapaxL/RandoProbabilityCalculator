using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class RandomFill : Algorithm
    {
        int ocCount = 0;

        public override Dictionary<string, long> Shuffle(Outcome outcome)
        {
            Console.WriteLine("starting random");
            var compiled = SubShuffle(0, outcome);
            Console.WriteLine("ending random");
            return compiled;
        }

        public Dictionary<string, long> SubShuffle(int curLoc, Outcome outcome)
        {
            if (outcome.UnplacedItems.Count == 0 || outcome.EmptyLocations.Count == 0)
            {
                ocCount++;
                if (ocCount % 1000 == 0) Console.WriteLine(ocCount);
                
                if (outcome.IsCompletable())
                    return CompileSingleOutcome(ocCount, outcome);
                else
                    return CompileSingleOutcome(ocCount, new FailedOutcome());
            }

            var compileds = new List<Dictionary<string, long>>();

            foreach (var item in outcome.UnplacedItems.Distinct())
            {
                var loc = outcome.World.ElementAt(curLoc);

                var compiled = SubShuffle(curLoc + 1, outcome.WithItemInLocation(item, loc.Key));
                compileds.Add(compiled);
                


                //foreach (var location in outcome.EmptyLocations)
                //{
                //    Shuffle(compiled, outcome.WithItemInLocation(item, location));
                //}
            }

            return CompileOutcomes(compileds);
        }
    }
}
