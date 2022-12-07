using HapaxTools;
using RandoProbabilityCalculator.ShuffleAlgExplorer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                new Item("C"),
                new Item("D"),
                new Item("x"),
                new Item("x"),
                //new Item("y"),
            };
            
            var reqA = new ItemReq(items[0]);
            var reqB = new ItemReq(items[1]);
            var reqC = new ItemReq(items[2]);
            var reqD = new ItemReq(items[3]);
            //var reqX1 = new ItemReq(items[4]);
            //var reqX2 = new ItemReq(items[5]);
            //var reqX = new ReqOr(reqX1, reqX2);
            //var req2X = new ReqAnd(reqX1, reqX2);

            //var locations = new List<Location>
            //{
            //    new Location(0, ReqExpr.None),
            //    new Location(1, reqX),
            //    new Location(2, new ReqOr(reqA, new ReqAnd(reqB, reqX))),
            //    new Location(3, new ReqOr(new ReqAnd(reqA, req2X), new ReqAnd(reqB, reqX), new ReqAnd(reqA, reqB))),
            //};

            //var locations = new List<Location>
            //{
            //    new Location(0, ReqExpr.None),
            //    new Location(1, reqB),
            //    new Location(2, reqA),
            //    new Location(3, reqA),
            //    //new Location(4, reqA),
            //    //new Location(5, reqA),
            //    // new Location(6, reqA),
            //};

            var locations = new List<Location>
            {
                new Location(0, Req.None),
                new Location(1, Req.None),
                //new Location(2, Req.None),
                new Location(2, reqA),
                new Location(3, new ReqAnd(reqA, reqB)),
                //new Location(5, new ReqAnd(reqA, reqC)),
                new Location(4, reqC),
                new Location(5, new ReqOr(reqD, reqC)),
            };

            //var locations = new List<Location>
            //{
            //    new Location(0, ReqExpr.None),
            //    new Location(1, reqA),
            //    new Location(2, reqA),
            //    new Location(3, reqA),
            //    new Location(4, reqA),
            //    //new Location(5, reqA),
            //    // new Location(6, reqA),
            //};

            var oc = new Outcome(items, locations);

            var bench = new Benchmark();
            bench.Add("Random", new RandomFill());
            bench.Add("AssumedExplorer", new AssumedFillExplorer(false));
            bench.Add("AssumedExplorer ignoring dupes", new AssumedFillExplorer(true));
            bench.Add("Assumed", new AssumedFill(false));
            bench.Add("Assumed ignoring junk", new AssumedFill(true));
            //bench.Add("Assumed SingleItem", new AssumedFill_SingleItem());
            //bench.Add("AssumedExplorer SingleItem", new AssumedFillExplorer_SingleItem());
            //bench.Add("Forward Simple Must Leave Open Locations", new ForwardFillSimple(false, true));
            //bench.Add("Forward Simple Must Open Locations", new ForwardFillSimple(true, false));
            bench.Run(oc);

            //var afillsep = assumed.ShufflePermsSeparately(oc);
            //Console.WriteLine("Separated Assumed results:");
            //foreach (var c in afillsep)
            //{
            //    var sb = new StringBuilder("[");
            //    foreach (var i in c.Key)
            //    {
            //        sb.Append(i);
            //    }
            //    sb.Append("]: ");
            //    sb.Append(c.Value.Sum(u => u.Value.Count));
            //    sb.Append(" outcomes");
            //    Algorithm.PrintResults(sb.ToString(), c.Value, false);
            //}
        }
    }
}
