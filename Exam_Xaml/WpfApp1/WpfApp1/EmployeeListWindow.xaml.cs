using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp1.Employee;

namespace WpfApp1
{
    /// <summary>
    /// EmployeeListWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EmployeeListWindow : Window
    {
        private readonly EmployeeListViewModel _mv = new EmployeeListViewModel();
        public EmployeeListWindow()
        {
            InitializeComponent();
            DataContext = _mv;
        }
    }
}
