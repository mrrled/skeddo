using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace newUI.ViewModels
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool>? canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommandAsync(Func<Task> execute, Func<bool>? canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;
        
        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                await execute();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}