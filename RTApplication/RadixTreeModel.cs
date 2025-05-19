
using MyList;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System;
using System.Linq;
using System.CodeDom.Compiler;



namespace RTApplication
{    
    
        public class RadixTreeNode
        {
            public string Prefix { get; set; } // Хранит префикс
            public Dictionary<string, RadixTreeNode> Children { get; set; }
            public bool IsEndOfWord { get; set; }

            public RadixTreeNode(string prefix = "")
            {
                Prefix = prefix;
                Children = new Dictionary<string, RadixTreeNode>();
                IsEndOfWord = false;
            }
        }

        public class RadixTree
        {
            public RadixTreeNode Root { get; }

            public RadixTree()
            {
                Root = new RadixTreeNode();
            }

            public void Insert(string word)
            {
                Insert(Root, word);
            }

            private void Insert(RadixTreeNode node, string word)
            {
                if (string.IsNullOrEmpty(word))
                {
                    node.IsEndOfWord = true;
                    return;
                }

                string firstChar = word[0].ToString();

                // Если узел не содержит текущий символ, создаем новый узел
                if (!node.Children.ContainsKey(firstChar))
                {
                    node.Children[firstChar] = new RadixTreeNode(word);
                    node.Children[firstChar].IsEndOfWord = true;
                    return;
                }

                var childNode = node.Children[firstChar];
                string commonPrefix = GetCommonPrefix(childNode.Prefix, word);

                if (commonPrefix.Length < childNode.Prefix.Length)
                {
                    // Разделяем узел
                    var newChild = new RadixTreeNode(childNode.Prefix.Substring(commonPrefix.Length))
                    {
                        IsEndOfWord = childNode.IsEndOfWord,
                        Children = childNode.Children
                    };
                    childNode.IsEndOfWord = false;
                    // Обновляем текущий узел
                    childNode.Prefix = commonPrefix;
                    childNode.Children = new Dictionary<string, RadixTreeNode> { { newChild.Prefix[0].ToString(), newChild } };

                    // Вставляем оставшуюся часть слова
                    Insert(childNode, word.Substring(commonPrefix.Length));
                }
                else if (commonPrefix.Length == childNode.Prefix.Length)
                {
                    // Если префикс совпадает, продолжаем вставку
                    Insert(childNode, word.Substring(commonPrefix.Length));
                }
                else
                {
                    // Если общий префикс меньше, чем длина текущего узла
                    var newChild = new RadixTreeNode(childNode.Prefix.Substring(commonPrefix.Length))
                    {
                        IsEndOfWord = childNode.IsEndOfWord,
                        Children = childNode.Children
                    };

                    childNode.Prefix = commonPrefix;
                    childNode.Children = new Dictionary<string, RadixTreeNode> { { newChild.Prefix[0].ToString(), newChild } };

                    // Вставляем оставшуюся часть слова
                    Insert(childNode, word.Substring(commonPrefix.Length));
                }
            }

            private string GetCommonPrefix(string str1, string str2)
            {
                int length = Math.Min(str1.Length, str2.Length);
                for (int i = 0; i < length; i++)
                {
                    if (str1[i] != str2[i])
                    {
                        return str1.Substring(0, i);
                    }
                }
                return str1.Substring(0, length); // Возвращаем общий префикс
            }

            public bool Search(string word)
            {
                var currentNode = Root;
                return Search(currentNode, word);
            }

            private bool Search(RadixTreeNode node, string word)
            {
                if (string.IsNullOrEmpty(word))
                {
                    return node.IsEndOfWord;
                }

                foreach (var child in node.Children)
                {
                    if (word.StartsWith(child.Value.Prefix))
                    {
                        return Search(child.Value, word.Substring(child.Value.Prefix.Length));
                    }
                }

                return false;
            }
        public MyList<string> MSearch(string word)
        {
            var currentNode = Root;
            var result = new MyList<string>();
            return MSearch(currentNode, word, result);
        }

        private MyList<string> MSearch(RadixTreeNode node, string word, MyList<string> result)
        {
            if (string.IsNullOrEmpty(word) && node.IsEndOfWord)
            {
                result.Add(node.Prefix);
            }

            foreach (var child in node.Children)
            {
                if (word.StartsWith(child.Value.Prefix))
                {
                result.Add(child.Value.Prefix);
                    result.AddRange(MSearch(child.Value, word.Substring(child.Value.Prefix.Length), result));
                }
            }
            return result;
        }

        public bool Delete(string word)
        {
            if (!Search(word))
                return false;
            Delete(Root, word);
            return true;
        }

        private bool Delete(RadixTreeNode node, string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                if (!node.IsEndOfWord)
                {
                    return false; // Слово не найдено
                }

                node.IsEndOfWord = false; // Удаляем слово
                return node.Children.Count == 0; // Если нет потомков, удаляем узел
            }
                            
            foreach (var child in node.Children)
            {
                if (word.StartsWith(child.Value.Prefix))
                {
                    bool shouldDeleteChild = Delete(child.Value, word.Substring(child.Value.Prefix.Length));

                    if (shouldDeleteChild)
                    {
                        node.Children.Remove(child.Key);
                        return node.Children.Count == 0 && !node.IsEndOfWord; // Удаляем узел, если он пуст
                    }
                }
            }
            return false; // Слово не найдено
        }
        // Очистка дерева
        public void Clear()
            {
                Root.Children.Clear();
            }
        }
    }


