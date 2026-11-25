
namespace WpfApp1
{
    public class EmployeeViewModel : ObservableObject
    {
        private string _employeeNo;
        private string _name;
        private string _email;
        private string _memo;
        private string _department;
        private DateTime? _hiredate;
        List<string> _departments;


        public EmployeeViewModel()
        {
            _employeeNo = "E-2026-0042";
            _name = "김민수";
            _email = "minsu.kim@example.com";
            _memo = "신입 공채 입사. 온보딩 교육 이수 완료.";
            _department = "개발1팀";
            _hiredate = new DateTime(2026,3,2);
            _departments = new List<string> {"개발1팀","개발2팀","인사팀"};
        }

        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    OnPropertyChanged(nameof(Summary));
                    OnPropertyChanged(nameof(CanSave));
                }
            }
        }

        public string EmployeeNo
        {
            get => _employeeNo;
            set
            {
                if (SetProperty(ref _employeeNo, value))
                {
                    OnPropertyChanged(nameof(Summary));
                }
            }
        }
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        public string Memo
        {
            get => _memo;
            set => SetProperty(ref _memo, value);
        }

        public string Department
        {
            get => _department;
            set
            {
                if (SetProperty(ref _department, value))
                {
                    OnPropertyChanged(nameof(Summary));
                }
            }
        }
        public IReadOnlyList<string> Departments
        {
            get => _departments;
        }

        public DateTime? HireDate
        {
            get => _hiredate;
            set => SetProperty(ref _hiredate, value);
        }

        public string Summary
        {
            get => $"편집 중: {_name} ({_employeeNo}) 부서: {Department}";
        }

        public bool CanSave
        {
            get => !string.IsNullOrWhiteSpace(Name);
        }
    }
}
