using RSOI_UI.ViewModels;

namespace RSOI_UI.Windows
{
    public partial class RegisterClientWindow
    {
        public RegisterClientWindow()
        {
            InitializeComponent();
            DataContext = new RegisterClientViewModel(this);
        }
    }
}
