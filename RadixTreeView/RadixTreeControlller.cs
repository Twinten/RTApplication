using MyList;
using RTApplication;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace RadixTreeView
{
    public class MainController
    {
        
        private readonly RadixTree model;
        MainWindow mainWindow;
        public MainController(MainWindow mainWindow)
        {            
            model = new RadixTree();          
            this.mainWindow = mainWindow;            

        }
         
        public void OnInsertRequested(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                mainWindow.ShowMessage("Введите слово для вставки.");
                return;
            }
            model.Insert(word);
            mainWindow.ShowMessage($"Слово \"{word}\" успешно вставлено.");
            mainWindow.ClearInput();
        }

        public void OnSearchRequested(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                mainWindow.ShowMessage("Введите слово для поиска.");
                return;
            }
            bool found = model.Search(word);
            mainWindow.ShowMessage(found ? $"Слово \"{word}\" найдено." : $"Слово \"{word}\" не найдено.");
            mainWindow.ClearInput();
        }

        public void OnDeleteRequested(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                mainWindow.ShowMessage("Введите слово для удаления.");
                return;
            }
            bool deleted = model.Delete(word);
            mainWindow.ShowMessage(deleted ? $"Слово \"{word}\" успешно удалено." : $"Слово \"{word}\" не найдено или не может быть удалено.");
            mainWindow.ClearInput();
        }

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
                model.Insert(word);
        }

        public MyList<string> GetAllWords()
        {
            var words = GetAllWordsFromNode(model.Root, "");
            return words;
        }
        public MyList<string> AutoComplete(string prefix)
        {
            var words = GetAllWordsFromNode(model.Root, "");
            var searchWords = new MyList<string>();
            foreach (var word in words)
                if (word.StartsWith(prefix))
                    searchWords.Add(word);
            return searchWords;
        }
        public RadixTreeNode FindNode(string prefix)
        {
            var currentNode = model.Root;
            foreach (char c in prefix)
            {
                if (!currentNode.Children.TryGetValue(c.ToString(), out var nextNode))
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
        public RadixTree GetTree() 
        {
            return model;
        }    
        
        public void Clear() 
        {
           model.Clear(); 
        }
    }
}
