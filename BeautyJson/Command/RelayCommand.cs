using System;
using System.Windows.Input;

namespace BeautyJson.Command
{
    public class RelayCommand:ICommand
    {
        private readonly Action<object> _execute;

        #region Declarations
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Memberfunction
        public RelayCommand(Action<object> execute)
        {
            _execute = execute;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke(parameter);
        }
        #endregion
    }
}