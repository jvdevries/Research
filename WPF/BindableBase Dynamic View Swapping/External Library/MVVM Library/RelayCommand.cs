using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace MVVM_Util
{
    public class RelayCommand : ICommand
    {
        #region Fields 

        private readonly Action<object> _execute;
        public readonly Predicate<object> _canExecute;

        #endregion // Fields 

        #region Constructors

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors 

        #region ICommand Members 

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter) => _execute(parameter);

        #endregion // ICommand Members 
    }
}