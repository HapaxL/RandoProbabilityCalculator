using RandoProbabilityCalculator.ShuffleAlgExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Item = System.String;

namespace RandoProbabilityCalculator
{
    class Program
    {
        //static List<Check> GetChecks(Area area)
        //{
        //    var checks = new List<Check>();

        //    foreach (var check in area.Checks)
        //    {
        //        checks.Add(new Check(check, area.Req));
        //    }

        //    foreach (var sub in area.SubAreas)
        //    {
        //        var subchecks = GetChecks(sub);
        //        checks.AddRange(subchecks);
        //    }

        //    return checks;
        //}

        //static int CollectKeys(List<Check> checks, int nbKeysLeft)
        //{
        //    var keyChecks = checks.Where(x => x.HasKey).ToList();
            
        //    var totalKeys = nbKeysLeft;
        //    var newKeys = totalKeys;

        //    do
        //    {
        //        totalKeys = newKeys;
        //        for (int i = 0; i < keyChecks.Count; i++)
        //        {
        //            if (keyChecks[i].Req <= totalKeys)
        //            {
        //                newKeys++;
        //                keyChecks.RemoveAt(i);
        //                i--;
        //            }
        //        }
        //    }
        //    while (newKeys != totalKeys);

        //    return totalKeys;
        //}

        //static IEnumerable<Check> GetValidChecks(List<Check> checks, int nbKeysLeft)
        //{
        //    var keys = CollectKeys(checks, nbKeysLeft);
        //    return checks.Where(x => !x.HasKey && x.Req <= keys);
        //}

        //static List<Check> BuildCheckList(Check[] initialCheckList, int[] keyCheckIds)
        //{
        //    var newChecks = new Check[initialCheckList.Length];

        //    for (int i = 0; i < initialCheckList.Length; i++)
        //    {
        //        newChecks[i] = new Check(initialCheckList[i].ID, initialCheckList[i].Req);
        //    }

        //    foreach (var id in keyCheckIds)
        //    {
        //        newChecks[id].HasKey = true;
        //    }

        //    return newChecks;
        //}

        static void Main(string[] args)
        {
            //var dungeon = new Area()
            //{
            //    SubAreas = new List<Area>()
            //    {
            //        new Area() { Req = 0, Checks = new int[] { 1, 2, 3, 4, } },
            //        new Area() { Req = 1, Checks = new int[] { 11, 12, 13, 14, 15, } },
            //        new Area() { Req = 2, Checks = new int[] { 21, 22, } },
            //        new Area() { Req = 3, Checks = new int[] { 31, 32, 33, } },
            //        new Area() { Req = 4, Checks = new int[] { 41, 42, 43, } },
            //    },
            //};

            var items = new List<Item>
            {
                new Item("A"),
                new Item("B"),
                new Item("x"),
                new Item("x"),

                //new Item("A"),
                //new Item("B"),
                //new Item("x"),
                //new Item("x"),
                //new Item("x"),
                //new Item("x"),
                // new Item("x"),
            };
            
            var reqA = new ItemReq(items[0]);
            var reqB = new ItemReq(items[1]);
            var reqX1 = new ItemReq(items[2]);
            var reqX2 = new ItemReq(items[3]);
            var reqX = new ReqOr(reqX1, reqX2);
            var req2X = new ReqAnd(reqX1, reqX2);

            var locations = new List<Location>
            {
                new Location(0, ReqExpr.None),
                new Location(1, reqX),
                new Location(2, new ReqOr(reqA, new ReqAnd(reqB, reqX))),
                new Location(3, new ReqOr(new ReqAnd(reqA, req2X), new ReqAnd(reqB, reqX), new ReqAnd(reqA, reqB))),
            };

            //var locations = new List<Location>
            //{
            //    new Location(0, ReqExpr.None),
            //    new Location(1, reqB),
            //    new Location(2, reqA),
            //    new Location(3, reqA),
            //    new Location(4, reqA),
            //    new Location(5, reqA),
            //    // new Location(6, reqA),
            //};

            var oc = new Outcome(items, locations);

            //var random = new RandomFill();
            var assumedE = new AssumedFillExplorer();
            var assumed = new AssumedFill();
            var single = new AssumedFill_SingleItem();

            //var rcompiled = random.Shuffle(oc);
            var aecompiled = assumedE.Shuffle(oc);
            var acompiled = assumed.Shuffle(oc);
            var scompiled = single.Shuffle(oc);

            var failedOutcomeString = Outcome.Failed.GetWorldString(0);

            //Console.WriteLine();
            //Console.WriteLine("Random:");
            //var rtotal = rcompiled.Values.Sum();
            //var rtotalSuccesses = rtotal - rcompiled[failedOutcomeString];
            //foreach (var c in rcompiled)
            //{
            //    if (c.Key == failedOutcomeString)
            //    {
            //        Console.WriteLine($"{c.Key}: {c.Value} ({100.0 * c.Value / rtotal}% of total)");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"{c.Key}: {c.Value} ({100.0 * c.Value / rtotal}% of total, {100.0 * c.Value / rtotalSuccesses}% of successes)");
            //    }
            //}

            Console.WriteLine();
            Console.WriteLine("AssumedExplorer:");
            var aetotal = aecompiled.Values.Select(kvp => kvp.Value).Sum();
            var aetotalSuccesses = aetotal - aecompiled[failedOutcomeString].Value;
            foreach (var c in aecompiled)
            {
                if (c.Key == failedOutcomeString)
                {
                    Console.WriteLine($"{c.Key}: ({c.Value.Key}) {c.Value.Value} ({100.0 * c.Value.Value / aetotal}% of total)");
                }
                else
                {
                    Console.WriteLine($"{c.Key}: ({c.Value.Key}) {c.Value.Value} ({100.0 * c.Value.Value / aetotal}% of total, {100.0 * c.Value.Value / aetotalSuccesses}% of successes)");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Assumed:");
            var atotal = acompiled.Values.Select(kvp => kvp.Proportion).Sum();
            var atotalSuccesses = atotal - acompiled[failedOutcomeString].Proportion;
            foreach (var c in acompiled)
            {
                if (c.Key == failedOutcomeString)
                {
                    Console.WriteLine($"{c.Key}: ({c.Value.Count}) {c.Value.Proportion} ({100.0 * c.Value.Proportion / atotal}% of total)");
                    //foreach (var parent in c.Value.Parents)
                    //{
                    //    Console.WriteLine($"    {parent.Key}: {parent.Value}");
                    //}
                }
                else
                {
                    Console.WriteLine($"{c.Key}: ({c.Value.Count}) {c.Value.Proportion} ({100.0 * c.Value.Proportion / atotal}% of total, {100.0 * c.Value.Proportion / atotalSuccesses}% of successes)");
                    //foreach (var parent in c.Value.Parents)
                    //{
                    //    Console.WriteLine($"    {parent.Key}: {parent.Value}");
                    //}
                }
            }

            Console.WriteLine();
            Console.WriteLine("Assumed SingleItem:");
            var stotal = scompiled.Values.Select(kvp => kvp.Proportion).Sum();
            var stotalSuccesses = stotal - scompiled[failedOutcomeString].Proportion;
            foreach (var c in scompiled)
            {
                if (c.Key == failedOutcomeString)
                {
                    Console.WriteLine($"{c.Key}: ({c.Value.Count}) {c.Value.Proportion} ({100.0 * c.Value.Proportion / stotal}% of total)");
                }
                else
                {
                    Console.WriteLine($"{c.Key}: ({c.Value.Count}) {c.Value.Proportion} ({100.0 * c.Value.Proportion / stotal}% of total, {100.0 * c.Value.Proportion / stotalSuccesses}% of successes)");
                }
            }

            Console.WriteLine();
        }
    }
}
