using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;

namespace MyRedBlackTree
{//用来遍历的迭代器

    internal abstract class Traversal<T> : IEnumerable<T> where T : IComparable<T>, IEquatable<T>
    {
        protected TreeNode<T> Root;
        public IEnumerator<T> GetEnumerator()
        {
            return Get(Root);
        }
        public Traversal(TreeNode<T> root)
        {
            Root = root;
        }
        protected abstract IEnumerator<T> Get(TreeNode<T> node);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    internal class LDR<T> : Traversal<T> where T : IComparable<T>, IEquatable<T>
    {
        public LDR(TreeNode<T> root) : base(root) { }
        protected override IEnumerator<T> Get(TreeNode<T> node)
        {
            if (!node) yield break;
            if (!node.Left && !node.Right)
            {
                yield return node.Value;
                yield break;
            }
            IEnumerator<T> left = Get(node.Left);
            while (left.MoveNext()) yield return left.Current;
            yield return node.Value;
            IEnumerator<T> right = Get(node.Right);
            while (right.MoveNext()) yield return right.Current;
        }
    }



    internal class DLR<T> : Traversal<T> where T : IComparable<T>, IEquatable<T>
    {
        public DLR(TreeNode<T> root) : base(root) { }
        protected override IEnumerator<T> Get(TreeNode<T> node)
        {
            if (!node) yield break;
            if (!node.Left && !node.Right)
            {
                yield return node.Value;
                yield break;
            }
            yield return node.Value;
            IEnumerator<T> left = Get(node.Left);
            while (left.MoveNext()) yield return left.Current;
            IEnumerator<T> right = Get(node.Right);
            while (right.MoveNext()) yield return right.Current;
        }
    }


    internal class LRD<T> : Traversal<T> where T : IComparable<T>, IEquatable<T>
    {
        public LRD(TreeNode<T> root) : base(root) { }
        protected override IEnumerator<T> Get(TreeNode<T> node)
        {
            if (!node) yield break;
            if (!node.Left && !node.Right)
            {
                yield return node.Value;
                yield break;
            }

            IEnumerator<T> left = Get(node.Left);
            while (left.MoveNext()) yield return left.Current;
            IEnumerator<T> right = Get(node.Right);
            while (right.MoveNext()) yield return right.Current;
            yield return node.Value;
        }

    }

    internal class Level<T> : Traversal<T> where T : IComparable<T>, IEquatable<T>
    {

        public Level(TreeNode<T> root) : base(root)
        { }
        protected override IEnumerator<T> Get(TreeNode<T> node)
        {
            if (!node) yield break;
            List<List<TreeNode<T>>> res = new List<List<TreeNode<T>>>();
            GetLevels(this.Root, 0, res);
            foreach (var level in res)
            {
                foreach (var _node in level)
                {
                    yield return _node.Value;
                }
            }
        }

        protected IEnumerator<TreeNode<T>> GetNode(TreeNode<T> node)
        {
            if (!node) yield break;
            List<List<TreeNode<T>>> res = new List<List<TreeNode<T>>>();
            GetLevels(this.Root, 0, res);
            foreach (var level in res)
            {
                foreach (var _node in level)
                {
                    yield return _node;
                }
            }
        }

        private void GetLevels(TreeNode<T> node, int n, List<List<TreeNode<T>>> res)
        {
            if (node == null) return;
            if (n == res.Count) res.Add(new List<TreeNode<T>>());
            res[n].Add(node);
            GetLevels(node.Left, n + 1, res);
            GetLevels(node.Right, n + 1, res);
        }
    }

    internal class LevelNode<T> : IEnumerable<TreeNode<T>> where T : IComparable<T>, IEquatable<T>
    {
        TreeNode<T> Root;
        public LevelNode(TreeNode<T> root)
        {
            Root = root;
        }

        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            return Get(Root);
        }

        protected IEnumerator<TreeNode<T>> Get(TreeNode<T> node)
        {
            if (!node) yield break;
            List<List<TreeNode<T>>> res = new List<List<TreeNode<T>>>();
            GetLevels(this.Root, 0, res);
            foreach (var level in res)
            {
                foreach (var _node in level)
                {
                    yield return _node;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private void GetLevels(TreeNode<T> node, int n, List<List<TreeNode<T>>> res)
        {
            if (node == null) return;
            if (n == res.Count) res.Add(new List<TreeNode<T>>());
            res[n].Add(node);
            GetLevels(node.Left, n + 1, res);
            GetLevels(node.Right, n + 1, res);
        }
    }
}
