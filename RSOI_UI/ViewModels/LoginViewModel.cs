using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RSOI_Data;
using RSOI_Data.Entities;
using RSOI_UI.Commands;

using LoginWindow = RSOI_UI.Windows.LoginWindow;
using MainWindow = RSOI_UI.Windows.MainWindow;

namespace RSOI_UI.ViewModels
{
    public class LoginViewModel:BaseViewModel
    {
        private LoginWindow _loginWindow;

        #region Presentation Properties

        public string Login { get; set; }

        #endregion
        
        #region Commands

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand => _loginCommand ??
                                                     (_loginCommand = new DelegateCommand(LoginExecute));

        #endregion

        public LoginViewModel(LoginWindow loginWindow)
        {
            _loginWindow = loginWindow;
        }
        
        #region Command Handlers

        private void LoginExecute()
        {
            var operatorCredentials = Credentials.GetCredentialsByLoginAndPassword(Login, _loginWindow.GetPasspord());

            if (operatorCredentials != null)
            {
                DepositManager.Instance.SetCurrentOperator(operatorCredentials);

                var mainWindow = new MainWindow();
                MessageBox.Show($"Добро пожаловать, {operatorCredentials.FIO.Trim()}", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                _loginWindow.Close();
                mainWindow.Show();
            }
            else
            {
                MessageBox.Show("Не удалось войти в систему", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
