using System;
using System.Windows.Input;

namespace BitcoinDesk.Helpers
{
	public class RelayCommand(Func<Task> execute, Func<bool>? canExecute = null) : ICommand
    {
        private readonly Func<Task> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

		public Func<bool> CanExecute1 { get; } = canExecute;

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) => CanExecute1 == null || CanExecute1();

		public async void Execute(object parameter) => await _execute();

		public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
