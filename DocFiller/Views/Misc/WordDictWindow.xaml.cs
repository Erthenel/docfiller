using System.Windows;

namespace DocFiller.Views.Misc
{
    public partial class WordDictWindow : Window
    {
        public WordDictWindow()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
