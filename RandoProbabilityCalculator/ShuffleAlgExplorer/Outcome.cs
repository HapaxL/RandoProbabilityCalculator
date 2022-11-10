using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Item = System.String;

namespace RandoProbabilityCalculator.ShuffleAlgExplorer
{
    public class Outcome
    {
        public List<Item> PlacedItems;
        public List<Item> UnplacedItems;
        public List<Location> EmptyLocations;
        public SortedList<Location, Item> World;

        public Outcome(List<Item> items, List<Location> locations)
        {
            PlacedItems = new List<Item>();
            UnplacedItems = items;
            EmptyLocations = locations;
            World = new SortedList<Location, Item>();
            foreach (var location in EmptyLocations)
            {
                World.Add(location, null);
            }
        }

        public Outcome(List<Item> placedItems, List<Item> unplacedItems, List<Location> locations, SortedList<Location, Item> world)
        {
            PlacedItems = placedItems;
            UnplacedItems = unplacedItems;
            EmptyLocations = locations;
            World = world;
        }

        public Outcome WithItemInLocation(Item item, Location location)
        {
            var placed = new List<Item>(PlacedItems);
            var unplaced = new List<Item>(UnplacedItems);
            var empty = new List<Location>(EmptyLocations);
            var world = new SortedList<Location, Item>(World);

            placed.Add(item);
            unplaced.Remove(item);
            empty.Remove(location);
            world[location] = item;

            return new Outcome(placed, unplaced, empty, world);
        }

        public bool IsCompletable()
        {
            var foundItems = new List<Item>(UnplacedItems);
            bool found = true;
            var unobtainedLocations = new Dictionary<Location, Item>();

            foreach (var kvp in World)
            {
                if (kvp.Value != null)
                {
                    unobtainedLocations.Add(kvp.Key, kvp.Value);
                }
            }

            while (found)
            {
                if (unobtainedLocations.Count == 0)
                    return true;

                found = false;
                var newUnobtained = new Dictionary<Location, Item>();

                foreach (var loc in unobtainedLocations)
                {
                    if (loc.Key.CanBeReachedWith(foundItems))
                    {
                        found = true;
                        foundItems.Add(loc.Value);
                    }
                    else
                    {
                        newUnobtained.Add(loc.Key, loc.Value);
                    }
                }

                unobtainedLocations = newUnobtained;
            }

            return false;
        }

        public virtual string GetWorldString(int count)
        {
            var sb = new StringBuilder("<");
            foreach (var kvp in World)
            {
                if (kvp.Value == null)
                {
                    sb.Append("_");
                }
                else
                {
                    sb.Append(kvp.Value);
                }
            }
            sb.Append(">");
            return sb.ToString();
        }

        public override string ToString()
        {
            return GetWorldString(0);
        }

        public static FailedOutcome Failed = new FailedOutcome();
    }

    public class FailedOutcome : Outcome
    {
        public FailedOutcome() : base(null, null, null, null) { }

        public override string GetWorldString(int count)
        {
            return "failed";
        }

        public override string ToString()
        {
            return GetWorldString(0);
        }
    }
}
