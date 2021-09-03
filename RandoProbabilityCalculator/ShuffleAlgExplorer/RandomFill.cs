using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class RandomFill : Algorithm
    {
        int ocCount = 0;

        public override Dictionary<string, int> Shuffle(Outcome outcome)
        {
            Console.WriteLine("starting random");
            var compiled = new Dictionary<string, int>();
            Shuffle(compiled, 0, outcome);
            Console.WriteLine("ending random");
            return compiled;
        }

        public void Shuffle(Dictionary<string, int> compiled, int curLoc, Outcome outcome)
        {
            if (outcome.UnplacedItems.Count == 0 || outcome.EmptyLocations.Count == 0)
            {
                ocCount++;
                if (ocCount % 1000 == 0) Console.WriteLine(ocCount);
                
                if (outcome.IsCompletable())
                    CompileOutcome(ocCount, compiled, outcome);
                else
                    CompileOutcome(ocCount, compiled, new FailedOutcome());

                return;
            }

            foreach (var item in outcome.UnplacedItems.Distinct())
            {
                var loc = outcome.World.ElementAt(curLoc);

                Shuffle(compiled, curLoc + 1, outcome.WithItemInLocation(item, loc.Key));


                //foreach (var location in outcome.EmptyLocations)
                //{
                //    Shuffle(compiled, outcome.WithItemInLocation(item, location));
                //}
            }
        }
    }
}
