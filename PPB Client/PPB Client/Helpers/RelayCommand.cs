using System;
using System.Windows.Input;

namespace PPB_Client.Helpers
{
    /// <summary>
    /// Relays commands from View to ViewModel.
    /// </summary>
    public class RelayCommand : ICommand
    {
        // Execute is a delegate which points to a method, it doesn't return a value.
        private readonly Action<object> execute;

        // Func is a delegate which points to a method, it returns a boolean value or reference.
        private readonly Func<object, bool> canExecute;

        /// <summary>
        /// Initializes a new instance of the RelayCommand class with the provided delegate.
        /// </summary>
        /// <param name="execute">Method which will be called by the command.</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the RelayCommand class with the provided delegate and predicate.
        /// </summary>
        /// <param name="execute">Method which will be called by the command.</param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute", "Execute cannot be null.");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// When changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Called when the command is invoked
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>True if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
            {
                return true;
            }

            return canExecute(parameter);
        }
    }
}
