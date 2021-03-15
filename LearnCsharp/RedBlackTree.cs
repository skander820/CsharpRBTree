using Microsoft.VisualBasic;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;

namespace MyRedBlackTree
{
    /// <summary>
    /// 红黑树
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedBlackTree<T> : IEquatable<RedBlackTree<T>> where T : IComparable<T>, IEquatable<T>, new()

    {
        public TreeNode<T> Root;

        public int Count { get; private set; }

        public static implicit operator bool(RedBlackTree<T> exsits) => exsits.Root != null;
        public static bool operator ==(RedBlackTree<T> tree, RedBlackTree<T> other) => tree.Equals(other);
        public static bool operator !=(RedBlackTree<T> tree, RedBlackTree<T> other) => !tree.Equals(other);

        #region Create
        public RedBlackTree() { }

        public RedBlackTree(List<T> list)
        {
            foreach (var item in list)
            {
                Add(item);
            }
        }

        public RedBlackTree(T[] array)
        {
            foreach (var item in array)
            {
                Add(item);
            }
        }
        #endregion

        #region Add
        public void Add(T value)
        {
            if (!Root)
            {
                Root = new TreeNode<T>(value, 0);
            }
            else
            {
                TreeNode<T> node = GetAddNode(Root, value);
                BalanceAfterAdd(node);

            }
            Count++;
        }
        /// <summary>
        /// 将需要插入的值置于对应位置并返回值所在节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        TreeNode<T> GetAddNode(TreeNode<T> node, T value)
        {

            int cmp = node.Value.CompareTo(value);
            if (cmp > 0)
            {
                if (!node.Left)
                {
                    node.Left = new TreeNode<T>(value);
                    return node.Left;
                }
                return GetAddNode(node.Left, value);
            }
            else if (cmp < 0)
            {
                if (!node.Right)
                {
                    node.Right = new TreeNode<T>(value);
                    return node.Right;
                }
                return GetAddNode(node.Right, value);
            }
            else
            {
                //如果已经存在这个节点 数量加一
                node.Add();
                return node;
            }


        }
        /// <summary>
        /// 插入后平衡红黑树结构
        /// </summary>
        /// <param name="node"></param>
        private void BalanceAfterAdd(TreeNode<T> node)
        {
            //当父节点颜色为红色,如果为黑色就不需要调整了
            while (node.Parent && node.Parent.Color == 1)
            {
                //父节点为祖父节点的左节点还是右节点
                //因为父节点颜色为红色所以必定有祖父节点且为黑色
                if (node.Parent == node.Parent.Parent.Left)
                {
                    var uncle = node.Parent.Parent.Right;
                    //如果叔叔节点为红色 相当于插入4-node
                    if (uncle && uncle.Color == 1)
                    {
                        //做ColorFlip以后祖父节点变红 所以需要跳转至祖父节点 进行下一步调整
                        node = node.Parent.Parent;
                        ColorFlip(node);
                    }
                    //如果叔叔为空或者黑色 相当于插入3-node
                    else
                    {   //如果为父节点的右节点 左旋
                        if (node == node.Parent.Right)
                        {
                            node = node.Parent;
                            RotateLeft(node);
                            //现在node是父节点的左节点了                             
                        }
                        //右旋
                        node = node.Parent.Parent;
                        RotateRight(node);
                    }
                }
                else
                {
                    var uncle = node.Parent.Parent.Left;
                    //如果叔叔节点为红色 相当于插入4-node
                    if (uncle && uncle.Color == 1)
                    {
                        //做ColorFlip以后祖父节点变红 所以需要跳转至祖父节点 进行下一步调整
                        node = node.Parent.Parent;
                        ColorFlip(node);
                    }
                    else
                    {
                        if (node == node.Parent.Left)
                        {
                            node = node.Parent;
                            RotateRight(node);
                            //现在node是父节点的右节点了          
                        }
                        //左旋
                        node = node.Parent.Parent;
                        RotateLeft(node);
                    }
                }
            }
            //根节点颜色始终为黑色
            Root.Color = 0;
        }
        #endregion

        #region Search
        public bool Exsist(T value)
        {
            if (!Root) return false;
            return Find(Root, value) != null;
        }

        private TreeNode<T> Find(TreeNode<T> node, T value)
        {

            if (!node) return null;
            int cmp = node.Value.CompareTo(value);
            if (cmp > 0)
            {
                return Find(node.Left, value);
            }
            else if (cmp < 0)
            {
                return Find(node.Right, value);
            }
            else
            {
                return node;
            }
        }
        #endregion

        #region Remove

        public bool Remove(T value)
        {

            //先确认树中有没有这个节点
            TreeNode<T> node = Find(Root, value);
            if (!node) return false;
            //节点的数量大于一
            if (node.Reduce()) return true;
            //如果没有子节点
            if (!node.Left && !node.Right)
            {
                //如果删除的节点不为Root
                if (node.Parent)
                    RemoveMax(node);
                else
                    Root = null;
            }
            else
            {
                if (node.Left)
                {
                    //用子节点小于自己的最大节点替代
                    node.SetValue(Max(node.Left));
                    //删除原替代节点
                    RemoveMax(node.Left);
                }
                else
                {
                    //用子节点大于自己的最小值替代
                    node.SetValue(Min(node.Right));
                    //删除原替代节点
                    RemoveMin(node.Right);
                }

            }
            Count--;
            return true;

        }

        /// <summary>
        /// 删除当前节点下的最大节点
        /// 删除一个黑节点会导致树不再平衡        
        /// 所以将所有情况转换成 待删除节点为红色末端节点 
        /// </summary>
        /// <param name="node"></param>
        private void RemoveMax(TreeNode<T> node)
        {
            //得到最大节点
            while (node.Right)
            {
                node = node.Right;
            }
            //node没有右子节点
            //如果有左子节点 node为黑 左子节点为红 否则不平
            if (node.Left)
            {
                RotateRight(node);
            }
            //此时 node为末端红节点        
            else
            {
                //如果node没有左子节点             
                RemoveFixUp(node);
            }

            //至此所有情况node皆为末端红节点删除node
            if (node == node.Parent.Right)
                node.Parent.Right = null;
            else
                node.Parent.Left = null;
        }
        private void RemoveMin(TreeNode<T> node)
        {

            while (node.Left)
            {
                node = node.Left;
            }

            //node没有左子节点
            //如果有右子节点 node为黑 右子节点为红 否则不平衡
            if (node.Right)
            {
                RotateLeft(node);
                //此时 node为末端红节点
            }
            else
            //如果node没有右子节点  
            {
                RemoveFixUp(node);
            }
            //至此所有情况node皆为末端红节点删除node
            if (node == node.Parent.Right)
            {
                node.Parent.Right = null;
            }
            else
            {
                node.Parent.Left = null;
            }
        }

        private void RemoveFixUp(TreeNode<T> node)
        {
            bool isLeft = node == node.Parent.Left;
            //如果node颜色为红不需要处理
            //如果颜色为黑
            if (node.Color == 0)
            {
                var brotherNode = isLeft ? node.Parent.Right : node.Parent.Left;
                //兄弟节点必然存在
                //如果兄弟节点为红
                if (brotherNode.Color == 1)
                {
                    //此时兄弟节点必有两个黑子节点 
                    //此时父节点必为黑
                    if (isLeft)
                    {
                        RotateLeft(node.Parent);
                    }
                    else
                    {
                        RotateRight(node.Parent);
                    }
                    ColorFlip(node.Parent);
                    //此时 node为末端红节点
                }
                else
                //如果兄弟节点为黑
                {
                    ColorFlip(node.Parent);
                    //如果兄弟节点有子节点
                    if (brotherNode.Left || brotherNode.Right)
                    {
                        if (isLeft)
                        {
                            if (!brotherNode.Right)
                            {
                                //为了保证后面colorFlip节点有两个子节点 
                                RotateRight(brotherNode);
                            }
                            RotateLeft(node.Parent);
                        }
                        else
                        {
                            if (!brotherNode.Left)
                            {
                                //为了保证后面colorFlip节点有两个子节点 
                                RotateLeft(brotherNode);
                            }
                            RotateRight(node.Parent);
                        }
                        //颜色复原
                        ColorFlip(node.Parent.Parent);
                    }
                    //root颜色可能改变 颜色复原
                    Root.Color = 0;
                    //此时 node为末端红节点
                }
            }
        }

        #endregion

        #region Tools
        /// <summary>
        /// 左旋
        /// </summary>
        /// <param name="node"></param>
        void RotateLeft(TreeNode<T> node)
        {
            if (!node.Right) return;
            TreeNode<T> p = node.Parent;
            TreeNode<T> t = node.Right;
            node.Right = t.Left;
            t.Left = node;
            t.Color = node.Color;
            node.Color = 1;
            if (!p)
            {
                Root = t;
                t.Parent = null;
            }
            else
            {
                if (p.Left == node)
                    p.Left = t;
                else
                    p.Right = t;
            }
        }
        /// <summary>
        /// 右旋
        /// </summary>
        /// <param name="node"></param>
        void RotateRight(TreeNode<T> node)
        {
            if (!node.Left) return;
            TreeNode<T> p = node.Parent;
            TreeNode<T> t = node.Left;
            node.Left = t.Right;
            t.Right = node;
            t.Color = node.Color;
            node.Color = 1;
            if (!p)
            {
                Root = t;
                t.Parent = null;
            }
            else
            {
                if (p.Left == node)
                    p.Left = t;
                else
                    p.Right = t;
            }
        }
        /// <summary>
        /// 颜色翻转 (2,3,4树中的节点上移下移)
        /// </summary>
        /// <param name="node"></param>
        void ColorFlip(TreeNode<T> node)
        {
            if (!node.Left || !node.Right) return;
            node.Color ^= 1;
            node.Left.Color ^= 1;
            node.Right.Color ^= 1;
        }
        /// <summary>
        /// 获取给予节点子节点中的最小节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns>最小节点</returns>
        public TreeNode<T> Min(TreeNode<T> node)
        {
            if (!node) throw new Exception("空节点没有最小值");
            while (node.Left) node = node.Left;
            return node;
        }

        /// <summary>
        /// 获取给予节点子节点中的最大节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns>最大节点</returns>
        public TreeNode<T> Max(TreeNode<T> node)
        {
            if (!node) throw new Exception("空节点没有最大值");
            while (node.Right) node = node.Right;
            return node;
        }

        /// <summary>
        /// 遍历每个子节点 一一判断是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals([AllowNull] RedBlackTree<T> other)
        {
            if (!other) return !Root;
            var thisEnum = this.LDR().GetEnumerator();
            var otherEnum = other.LDR().GetEnumerator();
            while (thisEnum.MoveNext() && otherEnum.MoveNext())
            {
                if (!thisEnum.Current.Equals(otherEnum.Current)) return false;
            }
            if (thisEnum.MoveNext() || otherEnum.MoveNext()) return false;
            return true;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Traversal
        public IEnumerable LDR()
        {
            return new LDR<T>(Root);
        }

        public IEnumerable DLR()
        {
            return new DLR<T>(Root);
        }
        public IEnumerable LRD()
        {
            return new LRD<T>(Root);
        }
        public IEnumerable LevelOrder()
        {
            return new Level<T>(Root);
        }
        public IEnumerable LevelOrderNode()
        {
            return new LevelNode<T>(Root);
        }
        #endregion

        #region Show
        /// <summary>
        /// 显示树(乱七八糟将就着用)
        /// </summary>
        /// <param name="gap">节点显示最小间隔</param>
        public void ShowTree(int gap = 1)
        {

            if (!Root)
            {
                Console.WriteLine("Null");
                return;
            }
            List<TreeNode<T>> res = new List<TreeNode<T>>();
            Traversal(Root, 0, res);
            int length = this.Max(Root).Value.ToString().Length;
            int deepth = (int)Math.Ceiling(Math.Log2(res.Count + 1));
            Console.WriteLine($"RedBlackTree Count:{Count} Deepth{deepth}");
            List<int> gapList = new List<int>() { 1 };
            List<int> firstGapList = new List<int>() { 0 };
            for (int i = 0; i < deepth; i++)
            {
                firstGapList.Insert(0, (int)(gapList[0] * (gap + length) - length) / 2 + length + firstGapList[0]);
                gapList.Insert(0, gapList[0] * 2);
            }
            for (int line = 0; line < deepth; line++)
            {
                for (int i = 0; i < (int)Math.Pow(2, line); i++)
                {
                    int index = (int)Math.Pow(2, line) + i - 1;
                    if (index >= res.Count) break;
                    if (res[index])
                    {
                        T value = res[index].Value;
                        int color = res[index].Color;
                        int count = res[index].Count;
                        Console.ForegroundColor = color == 1 ? ConsoleColor.Red : ConsoleColor.White;
                        Console.Write(new string(new char[i == 0 ? firstGapList[line] : (gapList[line] * (length + gap) - length)]));
                        Console.Write(string.Format(new string(new char[5] { '{', '0', ',', char.Parse(length.ToString()), '}' }), value));
                    }
                    else
                    {
                        Console.Write(new string(new char[i == 0 ? firstGapList[line] : (gapList[line] * (length + gap) - length)]));
                        Console.Write(new string(new char[length]));
                    }


                }
                Console.WriteLine("");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("--------------------------");
        }


        void Traversal(TreeNode<T> node, int k, List<TreeNode<T>> res)
        {
            if (node == null) return;
            if (k >= res.Count)
            {
                while (k > res.Count)
                {
                    res.Add(null);
                }
                res.Add(node);
            }
            else
            {
                res[k] = node;
            }
            Traversal(node.Left, k * 2 + 1, res);
            Traversal(node.Right, k * 2 + 2, res);
        }

        public void Debug(string title)
        {
            Console.WriteLine(title);
            ShowTree();

        }
        #endregion
    }

}
