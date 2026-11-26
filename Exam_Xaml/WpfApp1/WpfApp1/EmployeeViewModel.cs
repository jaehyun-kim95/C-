
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfApp1
{
    public partial class EmployeeViewModel : ObservableObject
    {
        //속성.
        [NotifyPropertyChangedFor(nameof(Summary))]
        [ObservableProperty] private string _employeeNo = "E-2026-0042";

        [NotifyPropertyChangedFor(nameof(Summary))]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [ObservableProperty] private string _name = "김민수";

        [ObservableProperty] private string _email = "minsu.kim@example.com";
        [ObservableProperty] private string _memo = "신입 공채 입사. 온보딩 교육 이수 완료.";

        [NotifyPropertyChangedFor(nameof(Summary))]
        [ObservableProperty] private string _department = "개발1팀";
        [ObservableProperty] private DateTime? _hireDate = new DateTime(2026, 3, 2);
        [ObservableProperty] private string _statusMessage = "준비";
        IReadOnlyList<string> _departments = new List<string> { "개발1팀", "개발2팀", "인사팀" };

        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        [ObservableProperty] private bool _isLoading;

        //public EmployeeViewModel()
        //{            
        //    //LoadCommand = new RelayCommand(Load);
        //    //SaveCommand = new RelayCommand(Save, CanSaveNow);
        //    //NavigateCommand = new RelayCommand<string>(Navigate);
        //}

        //public string Name
        //{
        //    get => _name;
        //    set
        //    {
        //        if (SetProperty(ref _name, value))
        //        {
        //            OnPropertyChanged(nameof(Summary));                    
        //            SaveCommand.NotifyCanExecuteChanged();                   
        //        }
        //    }
        //}

        //public string EmployeeNo
        //{
        //    get => _employeeNo;
        //    set
        //    {
        //        if (SetProperty(ref _employeeNo, value))
        //        {
        //            OnPropertyChanged(nameof(Summary));                    
        //        }
        //    }
        //}
        //public string Email
        //{
        //    get => _email;
        //    set => SetProperty(ref _email, value);
        //}
        //public string Memo
        //{
        //    get => _memo;
        //    set => SetProperty(ref _memo, value);
        //}

        //public string Department
        //{
        //    get => _department;
        //    set
        //    {
        //        if (SetProperty(ref _department, value))
        //        {
        //            OnPropertyChanged(nameof(Summary));
        //        }
        //    }
        //}
        public IReadOnlyList<string> Departments
        {
            get => _departments;
        }

        //public DateTime? HireDate
        //{
        //    get => _hiredate;
        //    set => SetProperty(ref _hiredate, value);
        //}
        //public string StatusMessage
        //{
        //    get => _statusMessage;
        //    set => SetProperty(ref _statusMessage, value);
        //}


        public string Summary
        {
            get => $"편집 중: {Name} ({EmployeeNo}) 부서: {Department}";
        }


        [RelayCommand(CanExecute = nameof(CanSaveNow))]
        private void Save()
        {
            StatusMessage = $"저장됨: {Name} ({EmployeeNo}) ({Email}) {DateTime.Now:HH:mm:ss}";
        }
        private bool CanSaveNow()
        {
            return !string.IsNullOrWhiteSpace(Name) && !IsLoading;
        }

        [RelayCommand(IncludeCancelCommand = true)]
        private async Task LoadAsync(CancellationToken ct)
        {
            IsLoading = true;
            try
            {
                StatusMessage = "불러오는 중 ...";
                await Task.Delay(2000,ct);

                EmployeeNo = "E-2026-0057";
                Name = "홍길동";
                Email = "test@gmail.com";
                Memo = "홍길동입니다.";
                Department = "인사팀";
                HireDate = new DateTime(2026, 11, 25);
                StatusMessage = $"불러옴: {Name} ({EmployeeNo}) {HireDate:yyyy-MM-dd}";
            }
            catch (OperationCanceledException ex)
            {
                StatusMessage = "취소됨";
            }
            finally {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void Navigate(string? menu) {
            if (menu is null) return;
            StatusMessage = $"{menu} 메뉴 선택";
        }

    }
}
