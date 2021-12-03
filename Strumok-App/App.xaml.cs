using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StrumokApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void StrumokApp_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageBoxResult mbResult = MessageBox.Show($"Збій в програмі. Рекомендовано закрити застосунок.\n{e.Exception}",
                "Закрити застосунок?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (mbResult == MessageBoxResult.Yes)
            {
                Shutdown();
            }
        }
    }
}
