using System;
using System.Collections.Generic;

namespace BalancedAbTrees
{
    public class AbNode<T> where T : IComparable<T>
    {
        protected readonly int A;
        protected readonly int B;

        //public AbNode<T> Parent;
        public List<AbNode<T>> Children;
        public List<T> Keys;

        public bool IsLeaf { get { return Children.Count == 0; } }
        public bool IsFull { get { return Keys.Count == B - 1; } }
        public bool IsOverflowing { get { return Keys.Count > B - 1; } }
        public bool IsUnderflowing{ get { return Keys.Count < A - 1; } }
        public bool HasMoreThanEnough { get { return Keys.Count > A - 1; } }

        public AbNode(int a, int b)
        {
            A = a;
            B = b;

            Children = new List<AbNode<T>>(b + 1); // allow overflowing nodes
            Keys = new List<T>(b - 1 + 1); // allow overflowing nodes
        }

        protected int BinarySearch(T key, int start, int end)
        {
            if (end < start)
            {
                return end;
            }

            var mid = (start + end) / 2;

            if (Keys[mid].CompareTo(key) == 0)
            {
                return mid;
            }
            else if (Keys[mid].CompareTo(key) < 0)
            {
                return BinarySearch(key, start, mid - 1);
            }
            else
            {
                return BinarySearch(key, mid + 1, end);
            }
        }

        protected int IterativeSearch(T key)
        {
            var i = 0;
            while (i < Keys.Count && Keys[i].CompareTo(key) < 0)
            {
                ++i;
            }

            return i;
        }

        public bool Contains(T key, out int pos)
        {
            pos = IterativeSearch(key);
            return pos < Keys.Count && Keys[pos].CompareTo(key) == 0;
        }

        public int Insert(T key)
        {
            Keys.Add(key);

            int i = Keys.Count - 2;
            while (i >= 0 && key.CompareTo(Keys[i]) < 0)
            {
                Keys[i + 1] = Keys[i];
                --i;
            }
            Keys[i+1] = key;
            return i + 1;
        }

        public override string ToString()
        {
            return string.Join(", ", Keys.ToArray());
        }
    }
}