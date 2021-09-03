using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public abstract class ReqExpr
    {
        public abstract bool Solve(List<Item> items);

        //public static ReqExpr FromString(string expr)
        //{

        //}

        public static NoReq None = new NoReq();
    }

    public class ReqAnd : ReqExpr
    {
        public ReqExpr[] Reqs;

        public ReqAnd(params ReqExpr[] reqs)
        {
            Reqs = reqs;
        }

        public override bool Solve(List<Item> items)
        {
            return Reqs.All(r => r.Solve(items));
        }
    }

    public class ReqOr : ReqExpr
    {
        public ReqExpr[] Reqs;

        public ReqOr(params ReqExpr[] reqs)
        {
            Reqs = reqs;
        }

        public override bool Solve(List<Item> items)
        {
            return Reqs.Any(r => r.Solve(items));
        }
    }

    public class ItemReq : ReqExpr
    {
        public Item Item;

        public ItemReq(Item item)
        {
            Item = item;
        }

        public override bool Solve(List<Item> items)
        {
            return items.Contains(Item);
        }
    }

    public class NoReq : ReqExpr
    {
        public override bool Solve(List<Item> items)
        {
            return true;
        }
    }
}
