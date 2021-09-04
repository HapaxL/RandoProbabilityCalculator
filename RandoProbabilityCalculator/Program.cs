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

        static void Main(string[] args)
        {
            var items = new List<Item>
            {
                //new Item("A"),
                //new Item("B"),
                //new Item("x"),
                //new Item("x"),

                new Item("a"),
                new Item("b"),
                new Item("x"),
                new Item("x"),
                new Item("x"),
                new Item("x"),
                // new item("x"),
            };
            
            var reqA = new ItemReq(items[0]);
            var reqB = new ItemReq(items[1]);
            var reqX1 = new ItemReq(items[2]);
            var reqX2 = new ItemReq(items[3]);
            var reqX = new ReqOr(reqX1, reqX2);
            var req2X = new ReqAnd(reqX1, reqX2);

            //var locations = new List<Location>
            //{
            //    new Location(0, ReqExpr.None),
            //    new Location(1, reqX),
            //    new Location(2, new ReqOr(reqA, new ReqAnd(reqB, reqX))),
            //    new Location(3, new ReqOr(new ReqAnd(reqA, req2X), new ReqAnd(reqB, reqX), new ReqAnd(reqA, reqB))),
            //};

            var locations = new List<Location>
            {
                new Location(0, ReqExpr.None),
                new Location(1, reqB),
                new Location(2, reqA),
                new Location(3, reqA),
                new Location(4, reqA),
                new Location(5, reqA),
                // new Location(6, reqA),
            };

            var oc = new Outcome(items, locations);

            var random = new RandomFill();
            var assumedE = new AssumedFillExplorer();
            var assumed = new AssumedFill();
            var single = new AssumedFill_SingleItem();
            var singleE = new AssumedFillExplorer_SingleItem();

            var rcompiled = random.Shuffle(oc);
            var aecompiled = assumedE.Shuffle(oc);
            var acompiled = assumed.Shuffle(oc);
            var scompiled = single.Shuffle(oc);
            var secompiled = singleE.Shuffle(oc);

            Algorithm.PrintResults("Random:", rcompiled);
            Algorithm.PrintResults("AssumedExplorer:", aecompiled);
            Algorithm.PrintResults("Assumed:", acompiled);
            Algorithm.PrintResults("Assumed SingleItem:", scompiled);
            Algorithm.PrintResults("AssumedExplorer SingleItem:", secompiled);
            
            Console.WriteLine();
        }
    }
}
