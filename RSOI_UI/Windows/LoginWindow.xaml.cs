using System.Windows;
using RSOI_UI.ViewModels;

namespace RSOI_UI.Windows
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }

        public string GetPasspord()
        {
            return _passwordBox.Password;
        }
    }
}
