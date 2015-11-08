using System;
using BalancedAbTrees;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BalancedAbTreesTests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void EmptyTree23()
        {
            var t = new AbTree<int>(2, 3);
            Assert.AreEqual(0, t.Count);
        }

        [TestMethod]
        public void EmptyTree23Finds()
        {
            var t = new AbTree<int>(2, 3);
            Assert.AreEqual(t.Contains(0), false);
            Assert.AreEqual(t.Contains(1), false);
            Assert.AreEqual(t.Contains(1000), false);
            Assert.AreEqual(t.Contains(-1), false);
            Assert.AreEqual(t.Contains(-1000), false);
        }

        [TestMethod]
        public void Tree23SimpleInsert()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(1000);
            t.Insert(1);
            Assert.AreEqual(2, t.Count);

            Assert.AreEqual(t.Contains(0), false);
            Assert.AreEqual(t.Contains(1), true);
            Assert.AreEqual(t.Contains(1000), true);
            Assert.AreEqual(t.Contains(-1), false);
            Assert.AreEqual(t.Contains(-1000), false);

            Assert.IsTrue(t.Root.Keys[0] < t.Root.Keys[1]);
        }

        [TestMethod]
        public void Tree24FullRoot()
        {
            var t = new AbTree<int>(2, 4);
            t.Insert(1000);
            t.Insert(500);
            t.Insert(1);
            Assert.AreEqual(3, t.Count);

            Assert.AreEqual(t.Contains(1), true);
            Assert.AreEqual(t.Contains(500), true);
            Assert.AreEqual(t.Contains(1000), true);

            Assert.IsTrue(t.Root.Keys[0] < t.Root.Keys[1]);
            Assert.IsTrue(t.Root.Keys[1] < t.Root.Keys[2]);

            Assert.AreEqual(1, t.NodesCount);
        }
         
        [TestMethod]
        public void Tree23FirstSplitRoot()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(1000);
            t.Insert(500);
            t.Insert(1);
            
            Assert.AreEqual(t.Contains(1), true);
            Assert.AreEqual(t.Contains(500), true);
            Assert.AreEqual(t.Contains(1000), true);

            //Assert.AreEqual(t.Root, t.Root.Children[0].Parent);

            Assert.AreEqual(1, t.Root.Keys.Count);
            Assert.AreEqual(3, t.NodesCount);
            Assert.AreEqual(3, t.Count);
        }

        [TestMethod]
        public void Tree23SimpleAddBelowRoot()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(1000);
            t.Insert(500);
            t.Insert(1);
            t.Insert(0);
            t.Insert(2000);

            Assert.AreEqual(t.Contains(0), true);
            Assert.AreEqual(t.Contains(1), true);
            Assert.AreEqual(t.Contains(500), true);
            Assert.AreEqual(t.Contains(1000), true);
            Assert.AreEqual(t.Contains(2000), true);

            //Assert.AreEqual(t.Root, t.Root.Children[0].Parent);
            //Assert.AreEqual(t.Root, t.Root.Children[1].Parent);

            Assert.AreEqual(5, t.Count);
            Assert.AreEqual(1, t.Root.Keys.Count);
            Assert.AreEqual(3, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SplitTwiceLeft()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(1000);
            t.Insert(500);
            t.Insert(1);
            t.Insert(0);
            t.Insert(2000);
            t.Insert(-1);

            Assert.AreEqual(t.Contains(-1), true);
            Assert.AreEqual(t.Contains(0), true);
            Assert.AreEqual(t.Contains(1), true);
            Assert.AreEqual(t.Contains(500), true);
            Assert.AreEqual(t.Contains(1000), true);
            Assert.AreEqual(t.Contains(2000), true);

            //Assert.AreEqual(t.Root, t.Root.Children[0].Parent);
            //Assert.AreEqual(t.Root, t.Root.Children[1].Parent);
            //Assert.AreEqual(t.Root, t.Root.Children[2].Parent);

            Assert.AreEqual(6, t.Count);
            Assert.AreEqual(2, t.Root.Keys.Count);
            Assert.AreEqual(4, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SplitLeftRightRoot()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(1000);
            t.Insert(500);
            t.Insert(1);
            t.Insert(0);
            t.Insert(2000);
            t.Insert(-1);
            t.Insert(5000);

            Assert.AreEqual(t.Contains(-1), true);
            Assert.AreEqual(t.Contains(0), true);
            Assert.AreEqual(t.Contains(1), true);
            Assert.AreEqual(t.Contains(500), true);
            Assert.AreEqual(t.Contains(1000), true);
            Assert.AreEqual(t.Contains(2000), true);
            Assert.AreEqual(t.Contains(5000), true);

            //Assert.AreEqual(t.Root, t.Root.Children[0].Parent);
            //Assert.AreEqual(t.Root, t.Root.Children[1].Parent);
            //Assert.AreEqual(t.Root.Children[0], t.Root.Children[0].Children[0].Parent);
            //Assert.AreEqual(t.Root.Children[0], t.Root.Children[0].Children[1].Parent);
            //Assert.AreEqual(t.Root.Children[1], t.Root.Children[1].Children[0].Parent);
            //Assert.AreEqual(t.Root.Children[1], t.Root.Children[1].Children[1].Parent);

            Assert.AreEqual(7, t.Count);
            Assert.AreEqual(1, t.Root.Keys.Count);
            Assert.AreEqual(7, t.NodesCount);
        }

        [TestMethod]
        public void Tree24SplitLeftRightRoot()
        {
            var t = new AbTree<int>(2, 4);
            t.Insert(1000);
            t.Insert(500);
            t.Insert(1);
            t.Insert(0);
            t.Insert(2000);
            t.Insert(-1);
            t.Insert(5000);

            Assert.AreEqual(t.Contains(-1), true);
            Assert.AreEqual(t.Contains(0), true);
            Assert.AreEqual(t.Contains(1), true);
            Assert.AreEqual(t.Contains(500), true);
            Assert.AreEqual(t.Contains(1000), true);
            Assert.AreEqual(t.Contains(2000), true);
            Assert.AreEqual(t.Contains(5000), true);

            Assert.AreEqual(7, t.Count);
            Assert.AreEqual(1, t.Root.Keys.Count);
            Assert.AreEqual(3, t.NodesCount);
        }

        [TestMethod]
        public void Tree23DeleteFromEmpty()
        {
            var t = new AbTree<int>(2, 4);
            t.Delete(500);

            Assert.AreEqual(t.Contains(500), false);

            Assert.AreEqual(0, t.Count);
            Assert.AreEqual(0, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SimpleDelete()
        {
            var t = new AbTree<int>(2, 4);
            t.Insert(1000);
            t.Insert(500);
            t.Delete(1000);
            t.Delete(500);

            Assert.AreEqual(t.Contains(500), false);
            Assert.AreEqual(t.Contains(1000), false);

            Assert.AreEqual(0, t.Count);
            Assert.AreEqual(null, t.Root);
            Assert.AreEqual(0, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SimpleDeleteMergeRight()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(100);
            t.Insert(50);
            t.Insert(1000);
            t.Delete(50);

            Assert.AreEqual(t.Contains(50), false);
            Assert.AreEqual(t.Contains(100), true);
            Assert.AreEqual(t.Contains(1000), true);

            Assert.AreEqual(2, t.Count);
            Assert.AreEqual(2, t.Root.Keys.Count);
            Assert.AreEqual(1, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SimpleDeleteBorrowRight()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(100);
            t.Insert(50);
            t.Insert(1000);
            t.Insert(2000);
            t.Delete(50);

            Assert.AreEqual(t.Contains(50), false);
            Assert.AreEqual(t.Contains(100), true);
            Assert.AreEqual(t.Contains(1000), true);
            Assert.AreEqual(t.Contains(2000), true);

            Assert.AreEqual(3, t.Count);
            Assert.AreEqual(1, t.Root.Keys.Count);
            Assert.AreEqual(3, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SimpleDeleteBorrowAndMergeRight()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(100);
            t.Insert(50);
            t.Insert(1000);
            t.Insert(2000);
            t.Delete(50);
            t.Delete(1000);

            Assert.AreEqual(t.Contains(50), false);
            Assert.AreEqual(t.Contains(100), true);
            Assert.AreEqual(t.Contains(1000), false);
            Assert.AreEqual(t.Contains(2000), true);

            Assert.AreEqual(2, t.Count);
            Assert.AreEqual(2, t.Root.Keys.Count);
            Assert.AreEqual(1, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SimpleDeleteBorrowMergePurgeRight()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(100);
            t.Insert(50);
            t.Insert(1000);
            t.Insert(2000);
            t.Delete(50);
            t.Delete(1000);
            t.Delete(100);
            t.Delete(2000);

            Assert.AreEqual(t.Contains(50), false);
            Assert.AreEqual(t.Contains(100), false);
            Assert.AreEqual(t.Contains(1000), false);
            Assert.AreEqual(t.Contains(2000), false);

            Assert.AreEqual(0, t.Count);
            Assert.AreEqual(null, t.Root);
            Assert.AreEqual(0, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SimpleDeleteMergeLeft()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(100);
            t.Insert(50);
            t.Insert(1000);
            t.Delete(1000);

            Assert.AreEqual(t.Contains(50), true);
            Assert.AreEqual(t.Contains(100), true);
            Assert.AreEqual(t.Contains(1000), false);

            Assert.AreEqual(2, t.Count);
            Assert.AreEqual(2, t.Root.Keys.Count);
            Assert.AreEqual(1, t.NodesCount);
        }

        [TestMethod]
        public void Tree23SimpleDeleteBorrowLeft()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(100);
            t.Insert(50);
            t.Insert(25);
            t.Insert(0);
            t.Delete(100);

            Assert.AreEqual(t.Contains(25), true);
            Assert.AreEqual(t.Contains(50), true);
            Assert.AreEqual(t.Contains(100), false);
            Assert.AreEqual(t.Contains(0), true);

            Assert.AreEqual(3, t.Count);
            Assert.AreEqual(1, t.Root.Keys.Count);
            Assert.AreEqual(3, t.NodesCount);
            Assert.IsFalse(t.Root.Children[1].IsUnderflowing);
        }

        [TestMethod]
        public void Tree23SimpleDeleteBorrowAndMergeLeft()
        {
            var t = new AbTree<int>(2, 3);
            t.Insert(100);
            t.Insert(50);
            t.Insert(1000);
            t.Insert(25);
            t.Delete(1000);
            t.Delete(100);

            Assert.AreEqual(t.Contains(25), true);
            Assert.AreEqual(t.Contains(50), true);
            Assert.AreEqual(t.Contains(100), false);
            Assert.AreEqual(t.Contains(1000), false);

            Assert.AreEqual(2, t.Count);
            Assert.AreEqual(2, t.Root.Keys.Count);
            Assert.AreEqual(1, t.NodesCount);
        }
    }
}
