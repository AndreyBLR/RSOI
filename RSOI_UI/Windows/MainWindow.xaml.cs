using System.Windows;
using RSOI_UI.ViewModels;

namespace RSOI_UI.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
