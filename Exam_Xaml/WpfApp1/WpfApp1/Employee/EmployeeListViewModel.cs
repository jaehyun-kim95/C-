using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace WpfApp1.Employee
{
    internal partial class EmployeeListViewModel:ObservableObject
    {
        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();

        public EmployeeListViewModel()
        {
            Employees.Add(new Employee { EmployeeNo = "E-2026-0042", Name = "김민수", Department= "개발1팀", HiraDate = new DateTime(2026, 3, 2), Email = "minsu@naver.com"});
            Employees.Add(new Employee { EmployeeNo = "E-2025-0031", Name = "박지영", Department = "인사팀", HiraDate = new DateTime(2025, 7, 14), Email = "jiyoung@naver.com" });
            Employees.Add(new Employee { EmployeeNo = "E-2024-0018", Name = "이서준", Department = "개발2팀", HiraDate = new DateTime(2024, 1, 8), Email = "seojun@naver.com" });
            Employees.Add(new Employee { EmployeeNo = "E-2023-0007", Name = "최유진", Department = "개발1팀", HiraDate = new DateTime(2023, 9, 25), Email = "yujin@naver.com" });
            Employees.Add(new Employee { EmployeeNo = "E-2022-0102", Name = "정하늘", Department = "인사팀", HiraDate = new DateTime(2022, 4, 11), Email = "haneil@naver.com" });
            Employees.Add(new Employee { EmployeeNo = "E-2021-0056", Name = "한도윤", Department = "개발2팀", HiraDate = new DateTime(2021, 11, 2), Email = "doyun@naver.com" });
        }
    }
}
