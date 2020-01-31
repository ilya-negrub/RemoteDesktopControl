using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientRdc.ViewModel
{
    internal class Command : ICommand
    {
        private Action Method;
        private Func<bool> methodCanExecute;

        public Command(Action method, Func<bool> canExecute = null)
        {
            Method = method;
            methodCanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;


        public void CanExecuteChangedInvoke() => CanExecuteChanged?.Invoke(this, null);

        public bool CanExecute(object parameter)
        {
            if (methodCanExecute == null)
                return Method != null;
            return methodCanExecute();
        }

        public void Execute(object parameter)
        {
            CanExecuteChanged?.Invoke(this, null);
            Method.Invoke();
        }
    }

    internal class Command<T> : ICommand
    {
        private Action<T> Method;
        private Func<bool> methodCanExecute;

        public Command(Action<T> method, Func<bool> canExecute = null)
        {
            Method = method;
            methodCanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;
        public void CanExecuteChangedInvoke() => CanExecuteChanged?.Invoke(this, null);

        public bool CanExecute(object parameter)
        {
            if (methodCanExecute == null)
                return Method != null;
            return methodCanExecute();
        }

        public void Execute(object parameter)
        {
            CanExecuteChanged?.Invoke(this, null);
            if (parameter != null)
                Method.Invoke((T)parameter);
        }
    }
}
