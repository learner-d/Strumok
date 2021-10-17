using Strumok_App.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Strumok_App.View
{
    /// <summary>
    /// Interaction logic for StrumokKeyInputWindow.xaml
    /// </summary>
    public partial class StrumokKeyInputWindow : Window
    {
        private StrumokKeyInputWindow() : this(new KeyInputVm())
        {}
        public StrumokKeyInputWindow(KeyInputVm viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void BtnApplyKey_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
