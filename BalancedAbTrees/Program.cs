using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BalancedAbTrees
{

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

            var sb = new StringBuilder();
            var queue = new Queue<AbNode<T>>();

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

            return;
        }

        static void Main(string[] args)
        {
            AbTree<ulong> t = new Ab24Tree();

            var statsTouched = 0;
            var statsChanged = 0;
            var nOps = 0;
            var N = 0UL;

            using (StreamWriter file = new StreamWriter(@"stats.txt"))
            {
                foreach (var op in ReadFile(args[0]))
                {
                    switch (op.Type)
                    {
                        case OpType.NewTree:
                            if (nOps > 0)
                            {
                                file.WriteLine("{0};{1};{2}", N, 1.0*statsTouched/nOps, 1.0*statsChanged/nOps);
                                file.Flush();
                            }

                            statsTouched = 0;
                            statsChanged = 0;
                            nOps = 0;
                            N = op.Key;
                            t.Clear();
                            continue;
                            break;
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
                    file.WriteLine("{0};{1};{2}", N, 1.0*statsTouched/nOps, 1.0*statsChanged/nOps);
                    file.Flush();
                }
            }
        }
    }
}
