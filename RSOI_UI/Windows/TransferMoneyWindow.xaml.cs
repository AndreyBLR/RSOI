using System.Windows;
using RSOI_Data.Entities;
using RSOI_UI.ViewModels;

namespace RSOI_UI.Windows
{
    /// <summary>
    /// Interaction logic for TransferMoneyToDeposit.xaml
    /// </summary>
    public partial class TransferMoneyWindow : Window
    {
        public TransferMoneyWindow(Client client, Deposit deposit)
        {
            InitializeComponent();
            DataContext = new TransferMoneyViewModel(this, client, deposit);
        }
    }
}
