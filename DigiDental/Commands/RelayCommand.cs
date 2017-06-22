using System;
using System.Windows.Input;

namespace DigiDental.Commands
{
    public class RelayCommand : ICommand
    {
        readonly Action execute;
        readonly Func<bool> canexecute;

        public RelayCommand(Action execute)
        {
            this.execute = execute;
        }
        public RelayCommand(Action execute, Func<bool> canexecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            this.execute = execute;
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
            execute();
        }
    }
}
