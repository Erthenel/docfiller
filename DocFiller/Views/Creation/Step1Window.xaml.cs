using System.Windows;
using DocFiller.Models;
using DocFiller.Utils;
using System.Collections.Generic;
using System;

namespace DocFiller.Views.Creation
{
    public partial class Step1Window : Window
    {
        public ProjectModel projectModel;

        public Step1Window(ProjectModel projectModel)
        {
            this.projectModel = projectModel;

            InitializeComponent();
        }

        private void templatePathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            bool? result = dlg.ShowDialog();

            if (result.HasValue && result == true)
            {
                templatePathTextBox.Text = templatePathTextBox.Text.EndsWith(";") || (templatePathTextBox.Text.Length == 0) ?
                    templatePathTextBox.Text + dlg.FileName : templatePathTextBox.Text + ";" + dlg.FileName;

                templatePathTextBox.ScrollToEnd();
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Use binding instead.
            projectModel.name = projectNameTextBox.Text.Trim();
            projectModel.templatePathText = templatePathTextBox.Text.Trim();
            projectModel.templatePaths = new HashSet<string>(
                !projectModel.templatePathText.Equals(string.Empty)? projectModel.templatePathText.Split(';') : new string[0]);
            projectModel.templateGroups = new List<string>() { "Общие закладки" };
            projectModel.templateMarkSpecGroup = new Dictionary<string, string>();
            projectModel.templateMarks = new Dictionary<string, string>();

            // TODO: Use another way for validation.
            List<string> errorEntries = new List<string>();
            if (projectModel.name.Length == 0)
            {
                errorEntries.Add("Имя проекта не указано.");
            }

            if (projectModel.templatePaths.Count == 0)
            {
                errorEntries.Add("Должен быть указан хотя бы один файл шаблона с закладками.");
            }

            try
            {
                if (errorEntries.Count != 0)
                {
                    InformDialogUtil.ShowErrorWithEntries(string.Empty, errorEntries);
                }
                else
                {
                    MicrosoftOfficeWrapper.ReadAllBookmarks(projectModel);

                    // Proceed to the next step.
                    Window nextWindow = new Step2Window(projectModel) { Owner = this };
                    nextWindow.Show();

                    Hide();
                }
            }
            catch (FormatException f)
            {
                InformDialogUtil.ShowError(f.Message);
            }
            catch (Exception)
            {
                InformDialogUtil.ShowError("Ошибка при открытии файла шаблона.");
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
