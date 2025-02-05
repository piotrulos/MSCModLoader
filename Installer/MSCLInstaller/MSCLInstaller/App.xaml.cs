using System;
using System.Threading;
using System.Windows;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex msclI;
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if DEBUG
            throw e.Exception;
#else
            MessageBoxResult result = MessageBox.Show($"Error:{Environment.NewLine}{Environment.NewLine}{e.Exception.Message}{Environment.NewLine}{Environment.NewLine}Please include 'Log.txt' file when reporting this issue.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Dbg.Log("!!! Unhandled Exception !!!", true, true);
            Dbg.Log(e.Exception.ToString());
            e.Handled = true;
#endif       
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool aIsNewInstance = false;
            msclI = new Mutex(true, "MSCLInstaller", out aIsNewInstance);
            if (!aIsNewInstance)
            {
                MessageBox.Show("MSCLInstaller is already running...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
    }
}
