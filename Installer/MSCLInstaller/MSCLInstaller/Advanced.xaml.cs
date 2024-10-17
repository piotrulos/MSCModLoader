using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows;
using System.Windows.Media;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for Advanced.xaml
    /// </summary>
    public partial class Advanced : Window
    {
        public Advanced()
        {
            InitializeComponent();
            switch (MD5FileHashes.MD5HashFile(Path.Combine(Storage.mscPath, "mysummercar_Data","Mono","mono.dll")))
            {
                case MD5FileHashes.mono64normal:
                    DebugStatus.Text = "Debugging is disabled (64-bit)";
                    DebugStatus.Foreground =  Brushes.Red;
                    ButtonSet(true, true);
                    break;
                case MD5FileHashes.mono64debug:
                    DebugStatus.Text = "Debugging is enabled (64-bit)";
                    DebugStatus.Foreground = Brushes.Green;
                    ButtonSet(true, false);

                    break;
                default:
                    DebugStatus.Text = "Unknown mono.dll version";
                    DebugStatus.Foreground = Brushes.Red;
                    ButtonSet(false, true);
                    break;
            }
        }

        private void ButtonSet(bool enabled, bool enabledbg)
        {
            DebugButton.IsEnabled = enabled;
            if (enabledbg)
                DebugButton.Content = "Enable Debugging";
            else
                DebugButton.Content = "Disable Debugging";
        }
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO:needs elevated priviledges (or just revert to old bat method)
            if(WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.AccountAdministratorSid))
                Environment.SetEnvironmentVariable("DNSPY_UNITY_DBG", "--debugger-agent=transport=dt_socket,embedding=1,server=y,address=127.0.0.1:56000,defer=y /m", EnvironmentVariableTarget.Machine);
            else
                MessageBox.Show("You need to have administrator priviledges to enable debugging. Please restart installer as Admin.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
