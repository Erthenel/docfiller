using DocFiller.Views;
using System.Windows;

namespace DocFiller
{
    public partial class App : Application
    {
        public App()
        {
            MainWindow mainView = new MainWindow();
            mainView.Show();
        }
    }
}
