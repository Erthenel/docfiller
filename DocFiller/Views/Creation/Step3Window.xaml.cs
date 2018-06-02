using DocFiller.Models;
using DocFiller.Utils;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DocFiller.Views.Creation
{
    public partial class Step3Window : Window
    {
        ProjectModel projectModel;

        public Step3Window(ProjectModel projectModel)
        {
            this.projectModel = projectModel;

            InitializeComponent();

            fillContent();
        }

        private void createProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string projectArchiveName = ZipWrapper.CreateProjectFileArchiveAndGetResultFilePath(projectModel, fbd.SelectedPath);

                InformDialogUtil.ShowInfo("Файл проекта создан:  \r\n" + projectArchiveName);

                Close();
                Owner.Close();
                Owner.Owner.Close();
                projectModel.openProjectDelegate(projectArchiveName);
            }
            else
            {
                InformDialogUtil.ShowWarning("Не указана директория для сохранения файла проекта.");
            }
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            Owner.Top = Top;
            Owner.Left = Left;
            Owner.Show();

            Close();
        }


        private void fillContent()
        {
            Thickness thickness = new Thickness(5, 5, 0, 0);

            infoStackPanel.Children.Add(new TextBlock() { Text = "Имя проекта: " + projectModel.name, Padding = thickness });
            infoStackPanel.Children.Add(new TextBlock() { Text = "Шаблон(ы): " + projectModel.templatePathText, Padding = thickness });
            infoStackPanel.Children.Add(new TextBlock() { Text = "Логические имена закладок: ", Padding = thickness });

            foreach (string templateKey in projectModel.templateMarks.Keys.ToArray())
            {
                infoStackPanel.Children.Add(new TextBlock() { Text = templateKey + " -> " + projectModel.templateMarks[templateKey], Padding = thickness });
            }
        }
    }
}
