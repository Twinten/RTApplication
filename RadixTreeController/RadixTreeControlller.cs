using System;
using Interfaces;
using RTApplication;

namespace RadixTreeController
{
    public class MainController
    {
        private readonly IView view;
        private readonly RadixTree model;

        public MainController(IView view, RadixTree model)
        {
            this.view = view;
            this.model = model;

            this.view.InsertRequested += OnInsertRequested;
            this.view.SearchRequested += OnSearchRequested;
            this.view.DeleteRequested += OnDeleteRequested;
        }

        private void OnInsertRequested(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                view.ShowMessage("Введите слово для вставки.");
                return;
            }
            model.Insert(word);
            view.ShowMessage($"Слово \"{word}\" успешно вставлено.");
            view.ClearInput();
        }

        private void OnSearchRequested(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                view.ShowMessage("Введите слово для поиска.");
                return;
            }
            bool found = model.Search(word);
            view.ShowMessage(found ? $"Слово \"{word}\" найдено." : $"Слово \"{word}\" не найдено.");
            view.ClearInput();
        }

        private void OnDeleteRequested(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                view.ShowMessage("Введите слово для удаления.");
                return;
            }
            bool deleted = model.Delete(word);
            view.ShowMessage(deleted ? $"Слово \"{word}\" успешно удалено." : $"Слово \"{word}\" не найдено или не может быть удалено.");
            view.ClearInput();
        }
    }
}
