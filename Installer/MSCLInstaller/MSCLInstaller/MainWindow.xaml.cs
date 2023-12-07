using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SelectGameFolder sgf;
        public MainWindow()
        {
            InitializeComponent();
            sgf = new SelectGameFolder();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = sgf;
            sgf.Init();
        }
    }
}
