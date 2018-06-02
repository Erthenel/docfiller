using System;
using System.Windows.Input;

namespace DocFiller.Utils
{
    class Command:ICommand
    {
            public Command(Action<object> action)
            {
                ExecuteDelegate = action;
            }

            #region Properties
            public Predicate<object> CanExecuteDelegate { get; set; }
            public Action<object> ExecuteDelegate { get; set; }
            #endregion

            #region ICommand Members
            public bool CanExecute(object parameter)
            {
                if (CanExecuteDelegate != null)
                {
                    return CanExecuteDelegate(parameter);
                }

                return true;
            }

            public void Execute(object parameter)
            {
                ExecuteDelegate?.Invoke(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
            #endregion
        }
    }
