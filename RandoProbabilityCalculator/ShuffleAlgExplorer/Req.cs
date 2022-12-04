using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public abstract class Req
    {
        public abstract bool Solve(List<Item> items);

        //public static ReqExpr FromString(string expr)
        //{

        //}

        public static NoReq None = new NoReq();

        public abstract IEnumerable<Item> GetRelatedItems();
    }

    public class ReqAnd : Req
    {
        public Req[] Reqs;

        public ReqAnd(params Req[] reqs)
        {
            Reqs = reqs;
        }

        public override bool Solve(List<Item> items)
        {
            return Reqs.All(r => r.Solve(items));
        }

        public override IEnumerable<Item> GetRelatedItems()
        {
            return Reqs.SelectMany(r => r.GetRelatedItems());
        }
    }

    public class ReqOr : Req
    {
        public Req[] Reqs;

        public ReqOr(params Req[] reqs)
        {
            Reqs = reqs;
        }

        public override bool Solve(List<Item> items)
        {
            return Reqs.Any(r => r.Solve(items));
        }

        public override IEnumerable<Item> GetRelatedItems()
        {
            return Reqs.SelectMany(r => r.GetRelatedItems());
        }
    }

    public class ItemReq : Req
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

        public override IEnumerable<Item> GetRelatedItems()
        {
            return new List<Item>() { Item };
        }
    }

    public class NoReq : Req
    {
        public override bool Solve(List<Item> items)
        {
            return true;
        }

        public override IEnumerable<Item> GetRelatedItems()
        {
            return new List<Item>();
        }
    }
}
