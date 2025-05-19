
using MyList;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System;
using System.Text.Encodings.Web;


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
                    if (word.StartsWith(child.Key.ToString()))
                    {
                        return Search(child.Value, word.Substring(child.Key.ToString().Length));
                    }
                }

                return false;
            }

            public bool Delete(string word)
            {
                return Delete(Root, word);
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
                    if (word.StartsWith(child.Key.ToString()))
                    {
                        bool shouldDeleteChild = Delete(child.Value, word.Substring(child.Key.ToString().Length));

                        if (shouldDeleteChild)
                        {
                            node.Children.Remove(child.Key);
                            return node.Children.Count == 0 && !node.IsEndOfWord; // Удаляем узел, если он пуст
                        }
                    }

                }
                return false; // Слово не найдено
            }



            public MyList<string> AutoComplete(string prefix)
            {
                var node = FindNode(prefix);
                return node != null ? GetAllWordsFromNode(node, prefix) : new MyList<string>();
            }
            public RadixTreeNode FindNode(string prefix)
            {
                var currentNode = Root;
                foreach (char c in prefix)
                {
                    if (!currentNode.Children.TryGetValue(prefix, out var nextNode))
                        return null;
                    currentNode = nextNode;
                }
                return currentNode;
            }

            public MyList<string> GetAllWordsFromNode(RadixTreeNode node, string prefix)
            {
                var words = new MyList<string>();
                if (node == null) return words;

                // Если текущий узел - конец слова, добавляем полный префикс
                if (node.IsEndOfWord)
                    words.Add(prefix + node.Prefix);
                

                // Рекурсивно собираем слова из дочерних узлов
                foreach (var child in node.Children)
                    words.AddRange(GetAllWordsFromNode(child.Value, prefix + node.Prefix));

                return words;
            }

            //private void GetWords(RadixTreeNode node, string currentPrefix, MyList<string> words)
            //{
            //    if (node.IsEndOfWord)
            //        words.Add(currentPrefix);

            //    foreach (var child in node.Children)
            //        GetWords(child.Value, currentPrefix, words);
            //}


            public MyList<string> GetAllWords()
            {
                var words = GetAllWordsFromNode(Root, "");
                return words;
            }

        //private void CollectWords(RadixTreeNode node, string currentPrefix, MyList<string> words)
        //{
        //    //// Если текущий узел является концом слова, добавляем в результат
        //    //if (node.IsEndOfWord)
        //    //{
        //    //    words.Add(currentPrefix);
        //    //}

        //    //// Рекурсивно обходим все дочерние узлы
        //    //foreach (var child in node.Children)
        //    //{
        //    //    CollectWords(child.Value, currentPrefix + child.Key, words);
        //    //}

        //}


        private static JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            public void SaveToJson(string filePath)
            {
                var words = GetAllWords();
                
                string json = JsonSerializer.Serialize(words, opt);
                File.WriteAllText(filePath, json);
            }

            public void LoadFromJson(string filePath)
            {
                string json = File.ReadAllText(filePath);
                var words = JsonSerializer.Deserialize<MyList<string>>(json, opt);
                Clear();
                foreach (var word in words)
                    Insert(word);
            }

            // Очистка дерева
            public void Clear()
            {
                Root.Children.Clear();
            }


        }
    }


