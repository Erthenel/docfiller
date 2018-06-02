using DocFiller.Models;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace DocFiller.Utils
{
    class MicrosoftOfficeWrapper
    {
        public static void UpdateBookmarkValuesAndSaveDocument(List<string> outputFilePaths, Dictionary<string, TextBox> markValueRefs)
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

            Application application = new Application();

            try
            {
                foreach (string outputFilePath in outputFilePaths)
                {
                    Document document = application.Documents.Open(outputFilePath, ReadOnly: false);

                    foreach (Bookmark bookmark in document.Bookmarks)
                    {
                        TextBox value;
                        markValueRefs.TryGetValue(bookmark.Name, out value);

                        // Replaces and removes cross-references for bookmark.
                        foreach (Field field in document.Fields)
                        {
                            if (field.Result.Text == bookmark.Range.Text || field.Result.Text.Equals(bookmark.Range.Text))
                            {
                                field.Select();
                                field.Delete();
                                application.Selection.TypeText(value.Text);

                                break;
                            }
                        }

                        // Replaces and removes bookmark.
                        bookmark.Range.Text = value.Text;
                    }

                    application.Documents.Save(NoPrompt: true);
                }
            }
            finally
            {
                application.Quit();
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
        }

        public static void ReadAllBookmarks(ProjectModel projectModel)
        {
            Application application = new Application();

            try
            {
                foreach (string templatePath in projectModel.templatePaths)
                {
                    if (templatePath.Length == 0)
                    {
                        continue;
                    }

                    Document document = application.Documents.Open(templatePath, ReadOnly: true);
                    string groupName = "Закладки для шаблона " + templatePath.Substring(templatePath.LastIndexOf('\\') + 1);
                    projectModel.templateGroups.Add(groupName);

                    if (document.Bookmarks.Count == 0)
                    {
                        throw new FormatException("Документ не содержит закладок: \r\n" + templatePath);
                    }

                    foreach (Bookmark bookmark in document.Bookmarks)
                    {
                        if (!projectModel.templateMarks.ContainsKey(bookmark.Name))
                        {
                            projectModel.templateMarks.Add(bookmark.Name, string.Empty);
                            projectModel.templateMarkSpecGroup.Add(bookmark.Name, groupName);
                        }
                        else
                        {
                            projectModel.templateMarkSpecGroup[bookmark.Name] = projectModel.templateGroups[0];
                        }
                    }

                    document.Close();
                }
            }
            finally
            {
                application.Quit();
            }
        }
    }
}
