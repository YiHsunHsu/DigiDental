using System;
using System.Windows.Input;

namespace DigiDental.Commands
{
    public class RelayCommandO : ICommand
    {
        readonly Func<bool> canexecute;
        private Action<object> executeO;

        public RelayCommandO(Action<object> execute)
        {
            this.executeO = execute;
        }

        public RelayCommandO(Action<object> execute, Func<bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            this.executeO = execute;
            this.canexecute = canexecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canexecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (canexecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }


        public bool CanExecute(object parameter)
        {
            return canexecute == null ? true : canexecute();
        }

        public void Execute(object parameter)
        {
            executeO(parameter);
        }
    }
}