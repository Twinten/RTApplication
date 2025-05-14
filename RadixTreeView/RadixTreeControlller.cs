using MyList;
using RTApplication;

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
        public RadixTree GetTree() 
        {
            return model;
        }
        public RadixTreeNode FindNode(string prefix)
        {
           return model.FindNode(prefix);
        }
        public MyList<string> GetAllWordsFromNode(RadixTreeNode node, string prefix)
        {
            return model.GetAllWordsFromNode(node, prefix);
        }
        public MyList<string> AutoComplete(string prefix) 
        {
            return model.AutoComplete(prefix);
        }
        public void SaveToJson(string filePath)
        {
            model.SaveToJson(filePath);
        }
        public void LoadFromJson(string filePath)
        {
            model.LoadFromJson(filePath);
        }
        public void Clear() 
        {
           model.Clear(); 
        }
    }
}
