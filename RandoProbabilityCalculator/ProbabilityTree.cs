using System;
using System.Collections.Generic;
using System.Text;

namespace RandoProbabilityCalculator
{
    public class PonderedTree<T>
    {
        public T Item;
        public PonderedTree<T> Parent;
        public int Depth;
        public Dictionary<PonderedTree<T>, double> Children;

        protected static PonderedTree<T> MakeNode(T item, PonderedTree<T> parent, int depth)
        {
            return new PonderedTree<T>()
            {
                Item = item,
                Parent = parent,
                Depth = depth,
                Children = new Dictionary<PonderedTree<T>, double>(),
            };
        }

        public static PonderedTree<T> Make(T item)
        {
            return MakeNode(item, null, 0);
        }

        public PonderedTree<T> AddChild(T item, double ponderation)
        {
            var node = MakeNode(item, this, Depth + 1);
            Children.Add(node, ponderation);
            return node;
        }

        protected void FillParentPath(T[] path)
        {
            path[Depth] = Item;
            if (Parent != null) FillParentPath(path);
        }

        public T[] GetPath()
        {
            var path = new T[Depth + 1];
            path[Depth] = Item;
            if (Parent != null) FillParentPath(path); 
            return path;
        }
    }
}
