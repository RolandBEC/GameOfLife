using System;
using System.Windows.Input;

namespace WPFClient1.Infrastructure
{
    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            Type type = typeof(T);
            if (parameter == null &&
                type.IsValueType &&
                !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return false;
            }

            return canExecute == null ? true : canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
