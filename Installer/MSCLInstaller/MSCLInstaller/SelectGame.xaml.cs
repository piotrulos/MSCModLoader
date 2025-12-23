using System.Windows.Controls;

namespace MSCLInstaller
{
    /// <summary>
    /// Interaction logic for SelectGame.xaml
    /// </summary>
    public partial class SelectGame : Page
    {
        MainWindow main;
        public SelectGame()
        {
            InitializeComponent();
        }
        public void Init(MainWindow m)
        {
            main = m;
        }

        private void MSC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Dbg.Log("Selected: My Summer Car");
            main.SelectGameFolderPage(Game.MSC);
        }

        private void MWC_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Dbg.Log("Selected: My Winter Car");
            main.SelectGameFolderPage(Game.MWC);
        }
    }
}
