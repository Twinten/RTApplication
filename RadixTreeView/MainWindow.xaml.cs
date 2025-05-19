using RTApplication;
using System;
using MyList;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace RadixTreeView
{
    public partial class MainWindow : Window
    {
        public MainController controller;
        private string lastProcessedText = "";
        private bool isSuggestionSelected = false;

        public MainWindow()
        {
            InitializeComponent();
            SuggestionsList.Visibility = Visibility.Collapsed;
            controller = new MainController(this);            
            UpdateTreeView();
        }
        private void UpdateTreeView()
        {
            var tree = controller.GetTree();
            TreeView.Items.Clear();
            if (tree.Root.Children.Count > 0)
            {
                var rootItem = new TreeViewItem
                {
                    Header = CreateNodeHeader(tree.Root),
                    Tag = tree.Root
                };
                BuildTreeItems(rootItem, tree.Root);
                TreeView.Items.Add(rootItem);
            }
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            controller.OnInsertRequested(InputTextBox.Text);
            UpdateTreeView();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            controller.OnSearchRequested(InputTextBox.Text);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            controller.OnDeleteRequested(InputTextBox.Text);
            UpdateTreeView();            
        }

        public void ShowMessage(string message)
        {
            OutputTextBox.Text = message;
        }

        public void ClearInput()
        {
            InputTextBox.Clear();
        }            

        private void BuildTreeItems(TreeViewItem parentItem, RadixTreeNode node)
        {
            foreach (var child in node.Children)
            {
                var childItem = new TreeViewItem
                {
                    Header = CreateNodeHeader(child.Value),
                    Tag = child.Value,
                    Foreground = child.Value.IsEndOfWord ? Brushes.Green : Brushes.Black
                };
                parentItem.Items.Add(childItem);
                BuildTreeItems(childItem, child.Value);
            }
        }

        private StackPanel CreateNodeHeader(RadixTreeNode node)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Children.Add(new TextBlock
            {
                Text = node.Prefix,
                FontWeight = FontWeights.Bold
            });

            if (node.IsEndOfWord)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = " (end)",
                    Foreground = Brushes.Green,
                    Margin = new Thickness(5, 0, 0, 0)
                });
            }

            return panel;
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isSuggestionSelected) // Игнорируем изменение текста при выборе подсказки
            {
                isSuggestionSelected = false;
                return;
            }

            string currentText = SearchBox.Text.Trim();
            if (currentText == lastProcessedText) return;

            if (currentText != SearchBox.Text.Trim()) return;

            lastProcessedText = currentText;
            UpdateSuggestions(currentText);
        }
        private void UpdateSuggestions(string input)
        {
            SuggestionsList.Items.Clear();

            if (string.IsNullOrWhiteSpace(input))
            {
                SuggestionsList.Visibility = Visibility.Collapsed;
                return;
            }

            // Берём последнее слово
            var words = input.Split(' ');
            string lastWord = words.Last();

            if (!string.IsNullOrEmpty(lastWord))
            {
                var suggestions = controller.AutoComplete(lastWord);             

                foreach (var suggestion in suggestions)
                    SuggestionsList.Items.Add(suggestion);

                SuggestionsList.Visibility = SuggestionsList.Items.Count > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }        
        private void SuggestionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SuggestionsList.SelectedItem == null) return;

            isSuggestionSelected = true; // Устанавливаем флаг

            string selectedSuggestion = SuggestionsList.SelectedItem.ToString();
            string currentText = SearchBox.Text;

            // Заменяем последнее слово
            var words = currentText.Split(' ');
            if (words.Length > 0)
                words[words.Length - 1] = selectedSuggestion;

            SearchBox.Text = string.Join(" ", words);

            // Устанавливаем курсор в конец и фокус
            SearchBox.CaretIndex = SearchBox.Text.Length;
            SearchBox.Focus();

            SuggestionsList.Visibility = Visibility.Collapsed;
        }    

        
          
        
        private void SaveDictionary_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Json files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = ".json",
                FileName = "radix_dictionary.json"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    controller.SaveToJson(saveDialog.FileName);
                    MessageBox.Show("Словарь успешно сохранен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void LoadDictionary_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Json files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (openDialog.ShowDialog() == true)
            {
                try
                {
                    controller.LoadFromJson(openDialog.FileName);
                    UpdateTreeView();
                    MessageBox.Show("Словарь успешно загружен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить весь словарь?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                controller.Clear();
                UpdateTreeView();
            }
        }

    }
}
