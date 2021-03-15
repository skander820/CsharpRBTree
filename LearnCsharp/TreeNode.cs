using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MyRedBlackTree
{


    public class TreeNode<T> : IComparable<TreeNode<T>>, IEquatable<TreeNode<T>> where T : IComparable<T>, IEquatable<T>
    {
        public T Value;
        public int Color;
        private int count = 1;
        private TreeNode<T> left;
        private TreeNode<T> right;
        public TreeNode<T> Parent;
        public int Count { get => count;private set => count = value; }

        public override string ToString()
        {
            return $"{Value},{Color},{left.Value},{right.Value}";
        }
        public TreeNode<T> Left
        {
            get => left; set
            {
                left = value;
                if (value) value.Parent = this;
            }
        }

        public TreeNode<T> Right
        {
            get => right; set
            {
                right = value;
                if (value) value.Parent = this;
            }
        }

        //重载bool
        public static implicit operator bool(TreeNode<T> exsits) => exsits != null;

        public TreeNode(T value, int color = 1)
        {
            Value = value;
            Color = color;
        }
        public void Add()
        {
            this.Count++;
        }

        public bool Reduce()
        {
            this.Count--;
            return Count > 0;
        }

        public void SetValue(TreeNode<T> node)
        {
            Count = node.Count;
            Value = node.Value;
        }

        public bool Equals([AllowNull] TreeNode<T> other)
        {
            return this.Value.Equals(other.Value) && this.count.Equals(other.count);
        }

        public int CompareTo([AllowNull] TreeNode<T> other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}
