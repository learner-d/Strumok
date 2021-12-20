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
        static App()
        {
            CosturaUtility.Initialize();
        }
        private void StrumokApp_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            string exceptionString = e.Exception.ToString();
            MessageBoxResult mbResult = MessageBox.Show($"Збій в програмі. Рекомендовано закрити застосунок.\n" +
                $"{exceptionString.Substring(Math.Min(exceptionString.Length, 200))}",
                "Закрити застосунок?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (mbResult == MessageBoxResult.Yes)
            {
                Shutdown();
            }
        }
    }
}
