using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* C# Trie implementation courtesy of Arnaldo Pérez Castaño - https://visualstudiomagazine.com/Articles/2015/10/20/Text-Pattern-Search-Trie-Class-NET.aspx?Page=1 */

namespace Laboratory
{
    class Program
    {
        static void Main()
        {
            var stream = new StreamReader("googleWordFrequencies.txt");
            var trie = new Trie();

            while (!stream.EndOfStream)
            {
                string[] word = new string[2];
                word = stream.ReadLine().Split(',');
                /*
                for (int i = 0; i < word.Length; i++)
                {
                    Console.WriteLine(word[i]);
                }
                */

                int wordWeight = 0;
                try
                {
                    //convert weight string from file into int
                    wordWeight = Int32.Parse(word[0]);
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
                trie.Insert(word[1], wordWeight);
            }

            Node prefix = trie.Prefix("the");
            Console.WriteLine(prefix.Value);
            Console.WriteLine(prefix.Weight);
            Console.WriteLine(prefix.Depth);
            foreach (var child in prefix.Children)
            {
                Console.WriteLine("child " + child.Value + " weight " + child.Weight);
            }
            

            Console.Read();
        }
    }

    public class Node
    {
        public char Value { get; set; }
        public List<Node> Children { get; set; }
        public Node Parent { get; set; }
        public int Depth { get; set; }
        public int Weight { get; set; }

        public Node(char value, int depth, int weight, Node parent)
        {
            Value = value;
            Children = new List<Node>();
            Depth = depth;
            Weight = weight;
            Parent = parent;
        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        public Node FindChildNode(char c)
        {
            foreach (var child in Children)
                if (child.Value == c)
                    return child;

            return null;
        }

        public void DeleteChildNode(char c)
        {
            for (var i = 0; i < Children.Count; i++)
                if (Children[i].Value == c)
                    Children.RemoveAt(i);
        }
    }

    public class Trie
    {
        private readonly Node _root;

        public Trie()
        {
            _root = new Node('^', 0, 0, null);
        }

        public Node Prefix(string s)
        {
            var currentNode = _root;
            var result = currentNode;

            foreach (var c in s)
            {
                currentNode = currentNode.FindChildNode(c);
                if (currentNode == null)
                    break;
                result = currentNode;
            }

            return result;
        }

        public bool Search(string s)
        {
            var prefix = Prefix(s);
            return prefix.Depth == s.Length && prefix.FindChildNode('$') != null;
        }

        public void Insert(string s, int weight)
        {
            var commonPrefix = Prefix(s);
            var current = commonPrefix;

            for (var i = current.Depth; i < s.Length; i++)
            {
                var newNode = new Node(s[i], current.Depth + 1, weight, current);
                current.Children.Add(newNode);
                current = newNode;
            }

            current.Children.Add(new Node('$', current.Depth + 1, weight, current));
        }

        public void Delete(string s)
        {
            if (Search(s))
            {
                var node = Prefix(s).FindChildNode('$');

                while (node.IsLeaf())
                {
                    var parent = node.Parent;
                    parent.DeleteChildNode(node.Value);
                    node = parent;
                }
            }
        }

    }


}
