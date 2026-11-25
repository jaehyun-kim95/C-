using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EmployeeViewModel employee = new EmployeeViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = employee;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"사번: {employee.EmployeeNo}, 이름: {employee.Name}, 이메일: {employee.Email}, 메모: {employee.Memo}, 부서: {employee.HireDate:yyyy-MM-dd}, " +
                $"입사일: {employee.HireDate}");           
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            employee.EmployeeNo = "E-2026-0057";
            employee.Name = "홍길동";
            employee.Email = "test@gmail.com";
            employee.Memo = "홍길동입니다.";
            employee.Department = "인사팀";
            employee.HireDate = new DateTime(2026,11,25);
        }
    }
}