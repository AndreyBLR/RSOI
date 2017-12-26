using System.Windows;
using RSOI_Data.Entities;
using RSOI_UI.ViewModels;

namespace RSOI_UI.Windows
{
    /// <summary>
    /// Interaction logic for WithdrawInterest.xaml
    /// </summary>
    public partial class WithdrawMoneyWindow : Window
    {
        public WithdrawMoneyWindow(Client client, Deposit deposit)
        {
            InitializeComponent();
            DataContext = new WithdrawMoneyViewModel(this, client, deposit);
        }
    }
}
