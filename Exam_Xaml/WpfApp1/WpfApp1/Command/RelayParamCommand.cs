using System.Windows.Input;

namespace WpfApp1
{
    public class RelayParamCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action<Object?> _execute;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }


        //생성자.
        public RelayParamCommand(Action<object?> Ac) {
            _execute = Ac;   
        }
    }
}
