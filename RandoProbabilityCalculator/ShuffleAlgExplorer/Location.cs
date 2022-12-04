using System;
using System.Collections.Generic;
using System.Text;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class Location : IComparable<Location>
    {
        public int Id;
        public Req Req;
        // public bool HasItem;
        // public Item Item;

		//public Location(ReqExpr req)
  //      {
  //          Req = req;
  //          HasItem = false;
  //      }

		//public void SetItem(Item item)
  //      {
  //          HasItem = true;
  //          Item = item;
  //      }

		public Location(int id, Req req)
        {
            Id = id;
            Req = req;
        }

		public bool CanBeReachedWith(List<Item> items)
        {
            return Req.Solve(items);
        }

        public IEnumerable<Item> GetRelatedItems()
        {
            return Req.GetRelatedItems();
        }

        public int CompareTo(Location other)
        {
            return Id.CompareTo(other.Id);
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
