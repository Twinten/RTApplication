
using MyList;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace RTApplication
{
    public class RadixTreeNode
    {
        public Dictionary<char, RadixTreeNode> Children { get; set; }
        public bool IsEndOfWord { get; set; }
        public string Prefix { get; set; } // Добавляем свойство для хранения префикса

        public RadixTreeNode(string prefix = "")
        {
            Children = new Dictionary<char, RadixTreeNode>();
            IsEndOfWord = false;
            Prefix = prefix;
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
            Insert(Root, word, 0, "");
        }

        private void Insert(RadixTreeNode node, string word, int index, string currentPrefix)
        {
            if (index == word.Length)
            {
                node.IsEndOfWord = true;
                node.Prefix = currentPrefix;
                return;
            }

            char currentChar = word[index];
            if (!node.Children.ContainsKey(currentChar))
            {
                node.Children[currentChar] = new RadixTreeNode(currentPrefix + currentChar);
            }

            Insert(node.Children[currentChar], word, index + 1, currentPrefix + currentChar);
        }


        public bool Search(string word)
        {
            var currentNode = Root;
            int index = 0;

            while (index < word.Length)
            {
                char currentChar = word[index];

                if (!currentNode.Children.ContainsKey(currentChar))
                {
                    return false;
                }

                currentNode = currentNode.Children[currentChar];
                index++;
            }

            return currentNode.IsEndOfWord;
        }

        public bool Delete(string word)
        {
            return Delete(Root, word, 0);
        }

        private bool Delete(RadixTreeNode currentNode, string word, int index)
        {
            if (index == word.Length)
            {
                if (!currentNode.IsEndOfWord)
                {
                    return false;
                }

                currentNode.IsEndOfWord = false;
                return currentNode.Children.Count == 0;
            }

            char currentChar = word[index];
            if (!currentNode.Children.ContainsKey(currentChar))
            {
                return false;
            }

            bool shouldDeleteCurrentNode = Delete(currentNode.Children[currentChar], word, index + 1);

            if (shouldDeleteCurrentNode)
            {
                currentNode.Children.Remove(currentChar);
                return currentNode.Children.Count == 0 && !currentNode.IsEndOfWord;
            }

            return false;
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
                if (!currentNode.Children.TryGetValue(c, out var nextNode))
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
                words.Add(prefix);

            // Рекурсивно собираем слова из дочерних узлов
            foreach (var child in node.Children)
                words.AddRange(GetAllWordsFromNode(child.Value, prefix + child.Key));

            return words;
        }

        private void GetWords(RadixTreeNode node, string currentPrefix, MyList<string> words)
        {
            if (node.IsEndOfWord)
                words.Add(currentPrefix);

            foreach (var child in node.Children)
                GetWords(child.Value, currentPrefix, words);
        }


        public MyList<string> GetAllWords()
        {
            var words = new MyList<string>();
            CollectWords(Root, "", words);
            return words;
        }

        private void CollectWords(RadixTreeNode node, string currentPrefix, MyList<string> words)
        {
            // Если текущий узел является концом слова, добавляем в результат
            if (node.IsEndOfWord)
            {
                words.Add(currentPrefix);
            }

            // Рекурсивно обходим все дочерние узлы
            foreach (var child in node.Children)
            {
                CollectWords(child.Value, currentPrefix + child.Key, words);
            }
        }



        public void SaveToJson(string filePath)
        {
            var words = GetAllWords();
            string json = JsonSerializer.Serialize(words);
            File.WriteAllText(filePath, json);
        }        

        public void LoadFromJson(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var words = JsonSerializer.Deserialize <MyList<string>>(json);
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
