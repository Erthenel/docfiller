using DocFiller.Models;
using DocFiller.Utils;
using DocFiller.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DocFiller.Views
{
    public partial class MainWindow : Window
    {
        // Stores only marks key/value elements for opened projects.
        Dictionary<string, Dictionary<string, TextBox>> openedProjectMarkStorage =
            new Dictionary<string, Dictionary<string, TextBox>>();

        // Stores only auxiliary elements for opened projects (checkboxes, projectFilePath, outputFilePath).
        Dictionary<string, Dictionary<string, FrameworkElement>> openedProjectAuxiliaryStorage =
            new Dictionary<string, Dictionary<string, FrameworkElement>>();

        // Stores only key/value word elements for opened projects (word dictionaries).
        Dictionary<string, Dictionary<string, ObservableCollection<String>>> openedProjectDictWordStorage =
            new Dictionary<string, Dictionary<string, ObservableCollection<String>>>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateNewProjectMethod(object sender, RoutedEventArgs e)
        {
            ProjectModel projectModel = new ProjectModel();
            projectModel.openProjectDelegate = openProjectFunction;

            Window creationView = new Creation.Step1Window(projectModel) { Owner = this };
            creationView.ShowDialog();
        }

        private void OpenProjectMethod(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            bool? result = dlg.ShowDialog();

            try
            {
                if (!(result.HasValue && result == true))
                {
                    InformDialogUtil.ShowWarning("Файл проекта не выбран.");
                }
                else if (!dlg.FileName.EndsWith(ZipWrapper.ProgramArhiveExtension))
                {
                    InformDialogUtil.ShowError("Данный файл не является файлом проекта.");
                }
                else if (openedProjectMarkStorage.ContainsKey(dlg.FileName))
                {
                    InformDialogUtil.ShowError("Данный файл проекта уже открыт в программе.");
                }
                else
                {
                    openProjectFunction(dlg.FileName);
                }
            }
            catch (Exception)
            {
                openedProjectMarkStorage.Remove(dlg.FileName);
                openedProjectAuxiliaryStorage.Remove(dlg.FileName);
                openedProjectDictWordStorage.Remove(dlg.FileName);

                InformDialogUtil.ShowError("Файл проекта повреждён.");
            }
        }

        private void openProjectFunction(string projectFilePath)
        {
            ProjectModel currentProjectModel = ZipWrapper.ReadProjectModelFromArchive(projectFilePath);
            fillTabControlForSelectedProject(currentProjectModel, projectFilePath);
        }

        private void СloseCurrentProjectMethod(object sender, RoutedEventArgs e)
        {
            TabItem selectedItem = (TabItem)openedProjectTabControl.SelectedItem;

            if (selectedItem != null)
            {
                openedProjectMarkStorage.Remove(selectedItem.Uid);
                openedProjectAuxiliaryStorage.Remove(selectedItem.Uid);
                openedProjectDictWordStorage.Remove(selectedItem.Uid);
                openedProjectTabControl.Items.Remove(openedProjectTabControl.SelectedItem);

                if (openedProjectTabControl.Items.Count == 0)
                {
                    openedProjectTabControl.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                InformDialogUtil.ShowWarning("Нет открытого проекта.");
            }
        }

        private void documentPathBrowseButton_Click()
        {
            TabItem selectedItem = ((TabItem)openedProjectTabControl.SelectedItem);
            Dictionary<string, FrameworkElement> auxiliaryRefs;
            openedProjectAuxiliaryStorage.TryGetValue(selectedItem.Uid, out auxiliaryRefs);

            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.SelectedPath))
            {
                FrameworkElement outputDocPath;
                auxiliaryRefs.TryGetValue("outputDocPath", out outputDocPath);
                ((TextBox)outputDocPath).Text = dlg.SelectedPath;
            }
            else
            {
                InformDialogUtil.ShowWarning("Путь не указан.");
            }
        }

        private void openDictWordButton_Click(string markKey, string markValue)
        {
            TabItem selectedItem = ((TabItem)openedProjectTabControl.SelectedItem);
            Dictionary<string, ObservableCollection<string>> dictWord;
            bool result = openedProjectDictWordStorage.TryGetValue(selectedItem.Uid, out dictWord);

            if (!result)
            {
                dictWord = new Dictionary<string, ObservableCollection<string>>();
                openedProjectDictWordStorage.Add(selectedItem.Uid, dictWord);
            }

            DictWordModel dictWordModel = new DictWordModel();
            dictWordModel.MarkKey = markKey;
            dictWordModel.dictWordFunctionDelegate = insertChosenWordFunction;
            dictWordModel.MarkValue = markValue;

            ObservableCollection<string> tmp;

            if (dictWord.TryGetValue(markKey, out tmp))
            {
                dictWordModel.DictWords = tmp;
            }
            else
            {
                dictWord[markKey] = new ObservableCollection<string>();
                dictWordModel.DictWords = dictWord[markKey];
            }

            Window wordDictionaryWindow = new Misc.WordDictWindow()
            {
                Owner = this,
                DataContext = new WordDictViewModel() { DictWordModel = dictWordModel }
            };

            wordDictionaryWindow.ShowDialog();
        }

        private void insertChosenWordFunction(string markKey, string chosenValue)
        {
            TabItem selectedItem = (TabItem)openedProjectTabControl.SelectedItem;
            Dictionary<string, TextBox> markValueRefs;
            openedProjectMarkStorage.TryGetValue(selectedItem.Uid, out markValueRefs);

            markValueRefs[markKey].Text = chosenValue;
        }

        private void createDocumentButton_Click()
        {
            try
            {
                TabItem selectedItem = ((TabItem)openedProjectTabControl.SelectedItem);
                Dictionary<string, TextBox> markValueRefs;
                Dictionary<string, FrameworkElement> auxiliaryRefs;
                openedProjectMarkStorage.TryGetValue(selectedItem.Uid, out markValueRefs);
                openedProjectAuxiliaryStorage.TryGetValue(selectedItem.Uid, out auxiliaryRefs);
                FrameworkElement outputDocPath;
                auxiliaryRefs.TryGetValue("outputDocPath", out outputDocPath);
                FrameworkElement projectFilePath;
                auxiliaryRefs.TryGetValue("projectFilePath", out projectFilePath);

                if (((TextBox)outputDocPath).Text.Length == 0)
                {
                    InformDialogUtil.ShowError("Не указан путь сохранения.");

                    return;
                }

                List<string> outputFilePaths = ZipWrapper.ExtractTemplateAndGetResultFilePaths(((TextBox)projectFilePath).Text, ((TextBox)outputDocPath).Text, selectedItem.Header.ToString());

                MicrosoftOfficeWrapper.UpdateBookmarkValuesAndSaveDocument(outputFilePaths, markValueRefs);

                // Clear TextBox Elements.
                foreach (var mvr in markValueRefs)
                {
                    bool? isChecked = ((CheckBox)auxiliaryRefs[mvr.Key]).IsChecked;
                    if (isChecked.Value == false)
                    {
                        mvr.Value.Text = string.Empty;
                    }
                }

                InformDialogUtil.ShowInfoWithEntries("Создание документа(ов) выполнено успешно:", outputFilePaths);
            }
            catch (Exception ex)
            {
                InformDialogUtil.ShowError("Ошибка на этапе создания документа: \r\n" + ex.ToString());
            }
        }

        private void OpenAboutMethod(object sender, RoutedEventArgs e)
        {
            Window aboutView = new Misc.AboutWindow() { Owner = this };
            aboutView.ShowDialog();
        }

        private void SaveProjectMethod(object sender, RoutedEventArgs e)
        {
            TabItem selectedItem = (TabItem)openedProjectTabControl.SelectedItem;
            if (selectedItem == null)
            {
                InformDialogUtil.ShowWarning("Нет открытого проекта.");

                return;
            }

            Dictionary<string, ObservableCollection<string>> dictWord;
            openedProjectDictWordStorage.TryGetValue(selectedItem.Uid, out dictWord);
            if (dictWord != null)
            {
                ZipWrapper.UpdateDictWordInProjectFileArchive(selectedItem.Uid, dictWord);

                InformDialogUtil.ShowInfo("Cловари для закладок успешно сохранены в файле проекта: " + selectedItem.Uid);
            }
        }

        private void ExitApplicationMethod(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void fillTabControlForSelectedProject(ProjectModel currentProjectModel, string projectFilePath)
        {
            Thickness defaultThickness = new Thickness(5, 5, 5, 0);
            Thickness checkBoxThickness = new Thickness(5, 0, 5, 5);

            Dictionary<string, TextBox> markValueRefs = new Dictionary<string, TextBox>();
            Dictionary<string, FrameworkElement> auxiliaryRefs = new Dictionary<string, FrameworkElement>();

            auxiliaryRefs.Add("projectFilePath", new TextBox() { Text = projectFilePath });

            // Fill UI with elements.
            Grid infoGrid = new Grid();
            int rowNumber = 0;

            foreach (string groupName in currentProjectModel.templateGroups)
            {
                infoGrid.RowDefinitions.Add(new RowDefinition());
                TextBlock groupBlock = new TextBlock()
                {
                    Text = groupName,
                    MinWidth = 25,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                groupBlock.Padding = defaultThickness;
                infoGrid.Children.Add(groupBlock);
                Grid.SetRow(groupBlock, rowNumber++);

                foreach (KeyValuePair<string, string> pair in currentProjectModel.templateMarks)
                {
                    if (!currentProjectModel.templateMarkSpecGroup[pair.Key].Equals(groupName))
                    {
                        continue;
                    }

                    infoGrid.RowDefinitions.Add(new RowDefinition());
                    TextBlock textBlock = new TextBlock()
                    {
                        Text = pair.Value,
                        MinWidth = 25,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    textBlock.Padding = defaultThickness;
                    infoGrid.Children.Add(textBlock);
                    Grid.SetRow(textBlock, rowNumber++);

                    infoGrid.RowDefinitions.Add(new RowDefinition());
                    TextBox textBox = new TextBox()
                    {
                        MinWidth = 200
                    };
                    markValueRefs.Add(pair.Key, textBox);
                    textBox.Margin = defaultThickness;
                    infoGrid.Children.Add(textBox);
                    Grid.SetRow(textBox, rowNumber++);

                    infoGrid.RowDefinitions.Add(new RowDefinition());
                    Button button = new Button()
                    {
                        Content = "...",
                        Height = 20,
                        Width = 20,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Command = new Command(arg => openDictWordButton_Click(pair.Key, pair.Value))
                    };
                    infoGrid.Children.Add(button);
                    button.Margin = checkBoxThickness;
                    Grid.SetRow(button, rowNumber);

                    infoGrid.RowDefinitions.Add(new RowDefinition());
                    CheckBox checkBox = new CheckBox()
                    {
                        IsChecked = false,
                        Height = 20,
                        Width = 20,
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    infoGrid.Children.Add(checkBox);
                    checkBox.Margin = checkBoxThickness;
                    Grid.SetRow(checkBox, rowNumber++);
                    auxiliaryRefs.Add(pair.Key, checkBox);
                }

                infoGrid.RowDefinitions.Add(new RowDefinition());
                Separator groupSeparator = new Separator();
                infoGrid.Children.Add(groupSeparator);
                groupSeparator.Margin = new Thickness(5, 10, 5, 10);
                Grid.SetRow(groupSeparator, rowNumber++);
            }

            infoGrid.RowDefinitions.Add(new RowDefinition());
            TextBlock infoTextBlock = new TextBlock()
            {
                Text = "Путь сохранения документа(ов) после вставки закладок"
            };
            infoTextBlock.Padding = defaultThickness;
            infoGrid.Children.Add(infoTextBlock);
            Grid.SetRow(infoTextBlock, rowNumber++);


            infoGrid.RowDefinitions.Add(new RowDefinition());
            TextBox outputDocPath = new TextBox()
            {
                MinWidth = 200,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            auxiliaryRefs.Add("outputDocPath", outputDocPath);
            infoGrid.Children.Add(outputDocPath);
            outputDocPath.Margin = defaultThickness;
            Grid.SetRow(outputDocPath, rowNumber++);


            infoGrid.RowDefinitions.Add(new RowDefinition());
            Button outputDocButton = new Button()
            {
                Content = "Обзор",
                Width = 60,
                HorizontalAlignment = HorizontalAlignment.Right,
                Command = new Command(arg => documentPathBrowseButton_Click())
            };
            infoGrid.Children.Add(outputDocButton);
            outputDocButton.Margin = new Thickness(0, 0, 5, 0);
            Grid.SetRow(outputDocButton, rowNumber++);


            infoGrid.RowDefinitions.Add(new RowDefinition());
            Separator separator2 = new Separator();
            infoGrid.Children.Add(separator2);
            separator2.Margin = defaultThickness;
            Grid.SetRow(separator2, rowNumber++);


            infoGrid.RowDefinitions.Add(new RowDefinition());
            Button createFinalDocButton = new Button()
            {
                Content = "Cоздать",
                Width = 120,
                HorizontalAlignment = HorizontalAlignment.Center,
                Command = new Command(arg => createDocumentButton_Click())
            };


            infoGrid.Children.Add(createFinalDocButton);
            createFinalDocButton.Margin = defaultThickness;
            Grid.SetRow(createFinalDocButton, rowNumber++);

            // Organize storage units.
            openedProjectMarkStorage.Add(projectFilePath, markValueRefs);
            openedProjectAuxiliaryStorage.Add(projectFilePath, auxiliaryRefs);

            Dictionary<string, ObservableCollection<string>> dictWord =
                (Dictionary<string, ObservableCollection<string>>)ZipWrapper.ReadDictWordFromArchive(projectFilePath);
            if (dictWord != null)
            {
                openedProjectDictWordStorage.Add(projectFilePath, dictWord);
            }

            openedProjectTabControl.Items.Add(new TabItem() { Header = currentProjectModel.name, Content = infoGrid, Uid = projectFilePath });
            openedProjectTabControl.SelectedIndex = openedProjectTabControl.Items.Count - 1;
            openedProjectTabControl.Visibility = Visibility.Visible;
        }
    }
}