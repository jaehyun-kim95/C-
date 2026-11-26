using System.Security.Policy;
using System.Windows.Input;

namespace WpfApp1
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged; // xaml에서 command를 추가한 목록

        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        // 생성자. 생성 시 메서드를 필드로 할당.
        public RelayCommand(Action Ac, Func<bool>? Fc = null)
        {
            _execute = Ac;
            _canExecute = Fc;
        }

        //command로 바인딩 되어있는 버튼이 실행됐을 때 execute 호출.
        public void Execute(object? parameter)
        {
            _execute();
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute is null ) { return true; } 
            else {
                return _canExecute.Invoke();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
