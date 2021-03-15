
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace MyRedBlackTree
{
 
    class Program
    {




        public static void Main(string[] args)
        {



            List<int> list = new List<int>();
            var randomizer = new Random(1);
            //DEMO
            //return;
            for (int i = 0; i < 10; i++)
            {
                int n = randomizer.Next(100);
                list.Add(n);
            }
            //通过List创建红黑树
            RedBlackTree<int> tree = new RedBlackTree<int>(list);
            //显示树结构
            tree.Debug("Create:");
            //判断两棵树是否相等
            RedBlackTree<int> tree2 = new RedBlackTree<int>(list);
            Console.WriteLine(tree == tree2);

            Console.WriteLine("中序遍历");
            foreach (var item in tree.LDR())
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("前序遍历");
            foreach (var item in tree.DLR())
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("后序遍历");
            foreach (var item in tree.LRD())
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("层序遍历");
            foreach (var item in tree.LevelOrder())
            {
                Console.WriteLine(item);
            }
            //是否存在
            Console.WriteLine(tree.Exsist(8));
            //逐个删除
            foreach (var item in list)
            {
                tree.Remove(item);
                tree.Debug($"Remove {item}:");
            }




        }
    }
}
