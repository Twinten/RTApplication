using System.Collections.Generic;

namespace RTApplication
{
    public class RadixTreeNode
    {
        public Dictionary<char, RadixTreeNode> Children { get; set; }
        public bool IsEndOfWord { get; set; }

        public RadixTreeNode()
        {
            Children = new Dictionary<char, RadixTreeNode>();
            IsEndOfWord = false;
        }
    }

    public class RadixTree
    {
        private readonly RadixTreeNode root;

        public RadixTree()
        {
            root = new RadixTreeNode();
        }

        public void Insert(string word)
        {
            var currentNode = root;
            int index = 0;

            while (index < word.Length)
            {
                char currentChar = word[index];

                if (!currentNode.Children.ContainsKey(currentChar))
                {
                    currentNode.Children[currentChar] = new RadixTreeNode();
                }

                currentNode = currentNode.Children[currentChar];
                index++;
            }

            currentNode.IsEndOfWord = true;
        }

        public bool Search(string word)
        {
            var currentNode = root;
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
            return Delete(root, word, 0);
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
    }
}
