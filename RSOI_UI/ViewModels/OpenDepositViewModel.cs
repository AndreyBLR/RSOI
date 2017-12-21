using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using RSOI_Data.Entities;
using RSOI_UI.Views;

namespace RSOI_UI.ViewModels
{
    public class OpenDepositViewModel:BaseViewModel
    {
        private Client _client;
        private DepositType _selectedDepositType;
        private OpenDeposit _openDepositWindow;
        private IList<DepositType> _depositTypeList;

        public string PassportId => _client.Passport.PassportId;
        public string PassportNumber => _client.Passport.SerialNumber;
        public string Name => _client.Passport.Name;
        public string LastName => _client.Passport.LastName;
        public string MiddleName => _client.Passport.MiddleName;
        public string Birthday => _client.Passport.Birthday.ToString("d");

        public IList<DepositType> DepositTypes
        {
            get => _depositTypeList;
            set
            {
                _depositTypeList = value;
                OnPropertyChanged();
            }
        }

        public DepositType SelectedDepositType {
            get => _selectedDepositType;
            set
            {
                _selectedDepositType = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged(_openDepositCommand);
            }
        }
        
        public OpenDepositViewModel(OpenDeposit openDepositWindow, Client client)
        {
            _client = client;
            _openDepositWindow = openDepositWindow;
            _depositTypeList = DepositType.GetListOfDepositTypes();
        }

        #region Commands

        private DelegateCommand _openDepositCommand;

        public DelegateCommand OpenDepositCommand => _openDepositCommand ??
                                                      (_openDepositCommand = new DelegateCommand(OpenDepositExecute, OpenDepositCanExecute));

        #endregion

        #region Command Handlers

        private bool OpenDepositCanExecute()
        {
            return SelectedDepositType != null;
        }

        private void OpenDepositExecute()
        {
            var deposit = DepositManager.Instance.OpenDeposit(_client, SelectedDepositType);

            var transferMoneyToDepositWindow = new TransferMoneyToDeposit();
            transferMoneyToDepositWindow.DataContext = new TransferMoneyToDepositViewModel(transferMoneyToDepositWindow, _client, deposit);

            transferMoneyToDepositWindow.ShowDialog();

            _openDepositWindow.Close();
        }

        #endregion
    }
}
