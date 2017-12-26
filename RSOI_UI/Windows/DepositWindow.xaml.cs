using System.Windows;
using RSOI_Data.Entities;
using RSOI_UI.ViewModels;

namespace RSOI_UI.Windows
{
    /// <summary>
    /// Interaction logic for DepositView.xaml
    /// </summary>
    public partial class DepositWindow : Window
    {
        public DepositWindow(Deposit deposit)
        {
            InitializeComponent();
            DataContext = new DepositViewModel(deposit);
        }
    }
}
