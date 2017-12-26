using RSOI_Data.Entities;
using RSOI_UI.ViewModels;

namespace RSOI_UI.Windows
{
    /// <summary>
    /// Interaction logic for NewDeposit.xaml
    /// </summary>
    public partial class OpenDepositWindow
    {
        public OpenDepositWindow(Client client)
        {
            InitializeComponent();
            DataContext = new OpenDepositViewModel(this, client);
        }
    }
}
