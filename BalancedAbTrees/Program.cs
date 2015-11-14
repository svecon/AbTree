using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BalancedAbTrees
{
    public class AbTree<T> where T : IComparable<T>
    {
        public class AbNode<TU> where TU : IComparable<TU>
        {
            protected readonly int A;
            protected readonly int B;

            //public AbNode<T> Parent;
            public List<AbNode<TU>> Children;
            public List<TU> Keys;

            public bool IsLeaf { get { return Children.Count == 0; } }
            public bool IsFull { get { return Keys.Count == B - 1; } }
            public bool IsOverflowing { get { return Keys.Count > B - 1; } }
            public bool IsUnderflowing { get { return Keys.Count < A - 1; } }
            public bool HasMoreThanEnough { get { return Keys.Count > A - 1; } }

            public AbNode(int a, int b)
            {
                A = a;
                B = b;

                Children = new List<AbNode<TU>>(b + 1); // allow overflowing nodes
                Keys = new List<TU>(b - 1 + 1); // allow overflowing nodes
            }

            protected int BinarySearch(TU key, int start, int end)
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

            protected int IterativeSearch(TU key)
            {
                var i = 0;
                while (i < Keys.Count && Keys[i].CompareTo(key) < 0)
                {
                    ++i;
                }

                return i;
            }

            public bool Contains(TU key, out int pos)
            {
                pos = IterativeSearch(key);
                return pos < Keys.Count && Keys[pos].CompareTo(key) == 0;
            }

            public int Insert(TU key)
            {
                Keys.Add(key);

                int i = Keys.Count - 2;
                while (i >= 0 && key.CompareTo(Keys[i]) < 0)
                {
                    Keys[i + 1] = Keys[i];
                    --i;
                }
                Keys[i + 1] = key;
                return i + 1;
            }

            public override string ToString()
            {
                return string.Join(", ", Keys.ToArray());
            }
        }

        public Dictionary<AbNode<T>, bool> WasRead = new Dictionary<AbNode<T>, bool>();
        public Dictionary<AbNode<T>, bool> WasChanged = new Dictionary<AbNode<T>, bool>();

        private class PassableInsert
        {
            public readonly T Key;
            public readonly AbNode<T> Left;
            public readonly AbNode<T> Right;

            public PassableInsert(T key, AbNode<T> left, AbNode<T> right)
            {
                Key = key;
                Left = left;
                Right = right;
            }
        }

        protected readonly int A;
        protected readonly int B;
        public AbNode<T> Root { get; protected set; }

        public int Count { get; protected set; }
        public int NodesCount { get; protected set; }

        public AbTree(int a, int b)
        {
            if (B < 2 * A - 1)
            {
                throw new ArgumentException("B must be at least 2A-1");
            }

            A = a;
            B = b;
        }

        protected AbNode<T> CreateNode()
        {
            ++NodesCount;
            return new AbNode<T>(A, B);
        }

        #region Find
        private bool RecursiveFind(AbNode<T> node, T key)
        {
            if (!WasRead.ContainsKey(node)) WasRead.Add(node, true);

            int i;
            if (node.Contains(key, out i))
            {
                return true;
            }

            if (node.IsLeaf)
            {
                return false;
            }

            return RecursiveFind(node.Children[i], key);
        }

        public bool Contains(T key)
        {
            if (Root == null)
            {
                return false;
            }

            return RecursiveFind(Root, key);
        }
        #endregion

        #region Insert
        private PassableInsert RecursiveInsert(AbNode<T> node, T key)
        {
            if (!WasRead.ContainsKey(node)) WasRead.Add(node, true);

            int i;
            if (node.Contains(key, out i)) return null;

            if (!node.IsLeaf)
            {
                var passable = RecursiveInsert(node.Children[i], key);

                if (passable == null) return null;

                i = node.Insert(passable.Key);
                node.Children.Insert(i + 1, passable.Right);
                if (!WasChanged.ContainsKey(node)) WasChanged.Add(node, true);
            }
            else
            {
                node.Insert(key);
                ++Count;
                if (!WasChanged.ContainsKey(node)) WasChanged.Add(node, true);
            }

            if (!node.IsOverflowing) return null;

            var sibling = CreateNode();
            //sibling.Parent = node.Parent;

            if (!WasRead.ContainsKey(sibling)) WasRead.Add(sibling, true);
            if (!WasChanged.ContainsKey(node)) WasChanged.Add(node, true);
            if (!WasChanged.ContainsKey(sibling)) WasChanged.Add(sibling, true);

            var keyToTop = node.Keys[B / 2];

            sibling.Keys.AddRange(node.Keys.Skip(1 + B / 2));
            node.Keys.RemoveRange(B / 2, node.Keys.Count - B / 2);

            if (!node.IsLeaf)
            {
                sibling.Children.AddRange(node.Children.Skip(1 + B / 2));
                node.Children.RemoveRange(node.Children.Count - (B + 1) / 2, (B + 1) / 2);

                //foreach (var child in sibling.Children) child.Parent = sibling;
            }

            return new PassableInsert(keyToTop, node, sibling);
        }

        public void Insert(T key)
        {
            if (Root == null)
            {
                Root = CreateNode();
            }

            var passable = RecursiveInsert(Root, key);
            if (passable == null) return;

            Root = CreateNode();
            Root.Insert(passable.Key);
            Root.Children.Add(passable.Left);
            Root.Children.Add(passable.Right);
            //passable.Left.Parent = Root;
            //passable.Right.Parent = Root;

            if (!WasChanged.ContainsKey(Root)) WasChanged.Add(Root, true);
        }
        #endregion

        #region Delete
        private T MaximumOfSubtree(AbNode<T> node)
        {
            if (node.IsLeaf)
            {
                return node.Keys[node.Keys.Count - 1];
            }

            return MaximumOfSubtree(node.Children[node.Children.Count - 1]);
        }

        private void RecursiveDelete(AbNode<T> node, T key)
        {
            if (!WasRead.ContainsKey(node)) WasRead.Add(node, true);

            int i;
            if (node.Contains(key, out i))
            {
                if (node.IsLeaf)
                {
                    node.Keys.RemoveAt(i);
                    --Count;

                    if (!WasChanged.ContainsKey(node)) WasChanged.Add(node, true);
                    return;
                }

                node.Keys[i] = MaximumOfSubtree(node.Children[i]);
                RecursiveDelete(node.Children[i], node.Keys[i]);
                if (!WasChanged.ContainsKey(node)) WasChanged.Add(node, true);
            }
            else
            {
                RecursiveDelete(node.Children[i], key);
            }

            var si = node.Children[i];
            if (!si.IsUnderflowing) return;

            if (!WasChanged.ContainsKey(si)) WasChanged.Add(si, true);
            if (!WasChanged.ContainsKey(node)) WasChanged.Add(node, true);

            if (node.Children.Count > i + 1) // right sibling exists
            {
                var sir = node.Children[i + 1];

                if (!WasChanged.ContainsKey(sir)) WasChanged.Add(sir, true);
                if (!WasRead.ContainsKey(sir)) WasRead.Add(sir, true);

                if (sir.HasMoreThanEnough) // borrowing
                {
                    si.Insert(node.Keys[i]);
                    if (sir.Children.Count > 0)
                    {
                        si.Children.Add(sir.Children[0]);
                        sir.Children.RemoveAt(0);
                    }

                    node.Keys[i] = sir.Keys[0];
                    sir.Keys.RemoveAt(0);
                }
                else // merging
                {
                    si.Insert(node.Keys[i]);
                    si.Keys.AddRange(sir.Keys);
                    si.Children.AddRange(sir.Children);

                    node.Keys.RemoveAt(i);
                    node.Children.RemoveAt(i + 1);

                    --NodesCount;
                }
            }
            else if (i > 0) // left sibling exists
            {
                var sir = node.Children[i - 1];

                if (!WasChanged.ContainsKey(sir)) WasChanged.Add(sir, true);
                if (!WasRead.ContainsKey(sir)) WasRead.Add(sir, true);

                if (sir.HasMoreThanEnough) // borrowing
                {
                    si.Insert(node.Keys[i - 1]);
                    if (sir.Children.Count > 0)
                    {
                        si.Children.Insert(0, sir.Children[sir.Children.Count - 1]);
                        sir.Children.RemoveAt(sir.Children.Count - 1);
                    }

                    node.Keys[i - 1] = sir.Keys[sir.Keys.Count - 1];
                    sir.Keys.RemoveAt(sir.Keys.Count - 1);
                }
                else // merging
                {
                    sir.Insert(node.Keys[i - 1]);
                    sir.Keys.AddRange(si.Keys);
                    sir.Children.AddRange(si.Children);

                    node.Keys.RemoveAt(i - 1);
                    node.Children.RemoveAt(i);

                    --NodesCount;
                }
            }
        }

        public void Delete(T key)
        {
            if (Root == null) return;

            RecursiveDelete(Root, key);

            if (Root.Children.Count == 1)
            {
                Root = Root.Children[0];
                --NodesCount;
            }
            else if (Root.IsLeaf && Root.Keys.Count == 0)
            {
                Root = null;
                --NodesCount;
            }
        }
        #endregion

        public void Clear()
        {
            Root = null;
            Count = 0;
            NodesCount = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var queue = new Queue<AbNode<T>>();

            queue.Enqueue(Root);
            if (Root == null) return "Empty tree";

            while (queue.Count > 0)
            {
                var n = queue.Dequeue();
                sb
                    .Append("[")
                    .Append(string.Join(" ", n.Keys))
                    .Append("] ")
                ;

                foreach (var item in n.Children)
                {
                    queue.Enqueue(item);
                }
            }
            return sb.ToString();
        }
    }


    enum OpType { NewTree, Insert, Delete }

    struct Op
    {
        public OpType Type;
        public ulong Key;

        public Op(OpType type, ulong key)
        {
            Key = key;
            Type = type;
        }
    }

    class Program
    {

        static IEnumerable<Op> ReadFile(string filename)
        {
            string line;
            StreamReader file = new StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {

                OpType type = OpType.NewTree;
                switch (line[0])
                {
                    case '#':
                        type = OpType.NewTree;
                        break;
                    case 'I':
                        type = OpType.Insert;
                        break;
                    case 'D':
                        type = OpType.Delete;
                        break;
                }

                ulong key = 0;
                for (int i = 2; i < line.Length; i++)
                {
                    if (line[i] < '0' || line[i] > '9') { break; }

                    key *= 10;
                    key += (ulong)(line[i] - '0');
                }

                yield return new Op(type, key);
            }

            file.Close();
        }

        static void TestTree<T>(AbTree<T> t) where T : IComparable<T>
        {
            if (t.Root == null) return;

            var queue = new Queue<AbTree<T>.AbNode<T>>();

            queue.Enqueue(t.Root);

            while (queue.Count > 0)
            {
                var n = queue.Dequeue();

                if (n.Keys.Count + 1 != n.Children.Count && n.Children.Count != 0)
                {
                    throw new Exception("Wrong number of keys and children");
                }

                foreach (var item in n.Children)
                {
                    queue.Enqueue(item);
                }

                if (n.Children.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < n.Keys.Count; i++)
                {
                    foreach (var child in n.Children[i].Keys)
                    {
                        if (n.Keys[i].CompareTo(child) < 0)
                        {
                            throw new Exception("Children on the left are higher");
                        }
                    }
                }

                for (var i = 0; i < n.Keys.Count; i++)
                {
                    foreach (var child in n.Children[i + 1].Keys)
                    {
                        if (n.Keys[i].CompareTo(child) > 0)
                        {
                            throw new Exception("Children on the right are lower");
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var t = new AbTree<ulong>(int.Parse(args[1]), int.Parse(args[2]));

            var statsTouched = 0;
            var statsChanged = 0;
            var nOps = 0;
            var n = 0UL;

            foreach (var op in ReadFile(args[0]))
            {
                switch (op.Type)
                {
                    case OpType.NewTree:
                        if (nOps > 0)
                        {
                            Console.WriteLine("{0};{1};{2}", n, 1.0 * statsTouched / nOps, 1.0 * statsChanged / nOps);
                        }

                        statsTouched = 0;
                        statsChanged = 0;
                        nOps = 0;
                        n = op.Key;
                        t.Clear();
                        continue;
                    case OpType.Insert:
                        t.Insert(op.Key);
                        //TestTree(t);
                        break;
                    case OpType.Delete:
                        t.Delete(op.Key);
                        //TestTree(t);
                        break;
                }

                ++nOps;
                statsTouched += t.WasRead.Count;
                statsChanged += t.WasChanged.Count;

                t.WasChanged.Clear();
                t.WasRead.Clear();
            }

            if (nOps > 0)
            {
                Console.WriteLine("{0};{1};{2}", n, 1.0 * statsTouched / nOps, 1.0 * statsChanged / nOps);
            }
        }
    }
}
