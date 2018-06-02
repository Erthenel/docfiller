using DocFiller.Models;
using DocFiller.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DocFiller.ViewModels
{
    class WordDictViewModel
    {
        public DictWordModel DictWordModel { get; set; }

        public string SelectedEntry { get; set; }

        public Command AddWordCommand { get; set; }
        public Command DeleteWordCommand { get; set; }

        public Command CancelOperationCommand { get; set; }
        public Command AcceptOperationCommand { get; set; }


        public WordDictViewModel()
        {
            AddWordCommand = new Command(arg => AddWordMethod());
            DeleteWordCommand = new Command(arg => DeleteWordMethod());

            CancelOperationCommand = new Command(arg => CancelOperationMethod(arg));
            AcceptOperationCommand = new Command(arg => AcceptOperationMethod(arg));
        }

        private void AddWordMethod()
        {
            if (SelectedEntry != null)
            {
                DictWordModel.DictWords.Add(new string(SelectedEntry.ToCharArray()));
            }
        }

        private void DeleteWordMethod()
        {
            DictWordModel.DictWords.Remove(SelectedEntry);
        }

        private void CancelOperationMethod(object Parameter)
        {
            Window objWindow = Parameter as Window;
            objWindow.Close();
        }

        private void AcceptOperationMethod(object Parameter)
        {
            DictWordModel.dictWordFunctionDelegate(DictWordModel.MarkKey, SelectedEntry);
            Window objWindow = Parameter as Window;
            objWindow.Close();
        }
    }
}
