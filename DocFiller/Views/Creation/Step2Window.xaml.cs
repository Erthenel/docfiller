using DocFiller.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DocFiller.Views.Creation
{
    public partial class Step2Window : Window
    {
        Dictionary<string, TextBox> keyRefs = new Dictionary<string, TextBox>();
        ProjectModel projectModel;

        public Step2Window(ProjectModel projectModel)
        {
            this.projectModel = projectModel;

            InitializeComponent();
            fillContent();
        }

        private void fillContent()
        {
            foreach (string templateKey in projectModel.templateMarks.Keys.ToArray())
            {
                TextBox textBox = new TextBox() { Width = 200 };
                keyRefs.Add(templateKey, textBox);

                infoWrapPanel.Children.Add(new TextBlock() { Text = templateKey, Width = 150 });
                infoWrapPanel.Children.Add(textBox);
            }
        }

        private void returnButton_Click(object sender, RoutedEventArgs e)
        {
            projectModel.templateMarks.Clear();

            Owner.Top = Top;
            Owner.Left = Left;
            Owner.Show();

            Close();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (string key in projectModel.templateMarks.Keys.ToArray())
            {
                string fullBookmarkName = keyRefs[key].Text.Trim();

                projectModel.templateMarks[key] = fullBookmarkName == string.Empty ? key: fullBookmarkName;
            }

            Window nextWindow = new Step3Window(projectModel) { Owner = this };
            nextWindow.Show();

            Hide();
        }
    }
}
