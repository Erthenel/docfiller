using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocFiller.Utils
{
    public class BaseNotifyPropetyChanged : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged(this, new PropertyChangedEventArgs("DisplayMember"));
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public bool HasErrors
        {
            get
            {
                return (this._errors.Count > 0);
            }
        }
        public IEnumerable GetErrors(string propertyName)
        {
            if (this._errors.ContainsKey(propertyName))
            {
                return this._errors[propertyName];
            }

            return null;
        }

        public bool IsValid
        {
            get { return !HasErrors; }

        }
        public void AddError(string propertyName, string error)
        {
            _errors[propertyName] = new List<string>() { error };

            NotifyErrorsChanged(propertyName);
            OnPropertyChanged("IsValid");
        }
        public void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
            }

            NotifyErrorsChanged(propertyName);
            OnPropertyChanged("IsValid");
        }

        public void NotifyErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}

