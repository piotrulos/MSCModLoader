using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for InstallProgress.xaml
    /// </summary>
    public partial class InstallProgress : Page
    {
        public InstallProgress()
        {
            InitializeComponent();
            LogBox.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddLog($"shit");
        }
        internal void DoSomeSHit()
        {
            AddLog($"DoSomeSHit");
        }
        private void AddLog(string log)
        {
            LogBox.AppendText($"{log}{Environment.NewLine}");
            LogBox.ScrollToEnd();
            Dbg.Log(log);
        }
    }
}
