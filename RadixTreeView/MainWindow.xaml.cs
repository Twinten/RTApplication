using System;
using System.Windows;


namespace RadixTreeView
{
    public partial class MainWindow : Window
    {
        public MainController controller;

        public MainWindow()
        {
            InitializeComponent();
            controller = new MainController(this);
        }

        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            controller.OnInsertRequested(InputTextBox.Text);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            controller.OnSearchRequested(InputTextBox.Text);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            controller.OnDeleteRequested(InputTextBox.Text);
        }

        public void ShowMessage(string message)
        {
            OutputTextBox.Text = message;
        }

        public void ClearInput()
        {
            InputTextBox.Clear();
        }
    }
}
