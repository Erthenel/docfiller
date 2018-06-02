using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFiller.Models
{
    class DictWordModel
    {
        public string MarkKey { get; set; }
        public string MarkValue { get; set; }
        public ObservableCollection<string> DictWords { get; set; }

        public delegate void DictWordFunctionDelegate(string markKey, string chosenValue);

        [NonSerialized]
        public DictWordFunctionDelegate dictWordFunctionDelegate;
    }
}
