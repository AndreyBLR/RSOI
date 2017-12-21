using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using RSOI_Data.Entities;
using RSOI_UI.Views;

namespace RSOI_UI.ViewModels
{
    public class TransferMoneyToDepositViewModel : BaseViewModel
    {
        private string _amount;
        private string _serialNumber;
        private string _fio;
        private Client _client;
        private Deposit _deposit;
        private TransferMoneyToDeposit _window;


        public string Name => _client.Passport.Name;
        public string LastName => _client.Passport.LastName;
        public string MiddleName => _client.Passport.MiddleName;
        public string Birthday => _client.Passport.Birthday.ToString("d");

        public string Number => _deposit.Number;
        public string CurrencyType => _deposit.CurrencyType.Name;

        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged(_tranferMoneyCommand);
            }
        }

        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                _serialNumber = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged(_tranferMoneyCommand);
            }
        }
        public string FIO
        {
            get => _fio;
            set
            {
                _fio = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged(_tranferMoneyCommand);
            }
        }

        public TransferMoneyToDepositViewModel(TransferMoneyToDeposit window, Client client, Deposit deposit)
        {
            _client = client;
            _deposit = deposit;
            _window = window;
        }

        #region Commands

        private DelegateCommand _tranferMoneyCommand;

        public DelegateCommand TranferMoneyCommand => _tranferMoneyCommand ??
                                                     (_tranferMoneyCommand = new DelegateCommand(TranferMoneyExecute));

        #endregion

        #region Command Handlers
        
        private void TranferMoneyExecute()
        {
            if (double.TryParse(Amount, out var amount))
            {
                if (DepositManager.Instance.AddToCurrentAccount(_deposit, amount, _serialNumber, _fio))
                {
                    if (MessageBox.Show("Средства зачислены", "Результат", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        _window.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка при зачислении средств", "Результат", MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Проверьте введённую сумму", "Результат", MessageBoxButton.OK);
            }
        }

        #endregion
    }
}
