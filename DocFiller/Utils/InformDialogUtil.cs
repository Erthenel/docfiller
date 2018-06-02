using System.Collections.Generic;
using System.Windows;

namespace DocFiller.Utils
{
    class InformDialogUtil
    {

        public static void ShowErrorWithEntries(string generalMessage, List<string> entries)
        {
            convertToString(entries);

            ShowError(generalMessage + "\n\r" + convertToString(entries));
        }

        public static void ShowInfoWithEntries(string generalMessage, List<string> entries)
        {
            convertToString(entries);

            ShowInfo(generalMessage + "\n\r" + convertToString(entries));
        }

        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowWarning(string text)
        {
            MessageBox.Show(text, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
  
        public static void ShowInfo(string text)
        {
            MessageBox.Show(text, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static string convertToString(List<string> list)
        {
            string result = string.Empty;

            foreach (string item in list)
            {
                result += item + "\n\r";
            }

            return result;
        }
    }
}
