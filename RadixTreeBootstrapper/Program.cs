using System;
using System.Windows;  // Добавлено пространство имен для WPF
using Interfaces; // Добавлено пространство имен RadixTreeInterfaces
using RadixTree;


namespace RadixTreeBootstrapper
{
    public partial class App : Application
    {
        [STAThread] // Атрибут для STAThread
        public static void Main()
        {
            var application = new App();
            var mainWindow = new MainWindow();
            application.Run(mainWindow);
        }
    }
}
