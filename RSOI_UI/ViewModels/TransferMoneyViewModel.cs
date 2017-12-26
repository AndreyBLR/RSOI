using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RSOI_Data;
using RSOI_Data.Entities;
using RSOI_UI.Commands;

using TransferMoneyWindow = RSOI_UI.Windows.TransferMoneyWindow;

namespace RSOI_UI.ViewModels
{
    public class TransferMoneyViewModel : BaseViewModel
    {
        private string _amount;
        private string _serialNumber;
        private string _fio;
        private Client _client;
        private Deposit _deposit;
        private TransferMoneyWindow _window;

        #region Presentation Properties

        public Client Client
        {
            get => _client;
            set
            {
                _client = value;
                OnPropertyChanged();
            }
        }

        public Deposit Deposit
        {
            get => _deposit;
            set
            {
                _deposit = value;
                OnPropertyChanged();
            }
        }

        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                _serialNumber = value;
                OnPropertyChanged();
            }
        }

        public string FIO
        {
            get => _fio;
            set
            {
                _fio = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public TransferMoneyViewModel(TransferMoneyWindow window, Client client, Deposit deposit)
        {
            Client = client;
            Deposit = deposit;
            _window = window;
        }

        #region Commands

        private DelegateCommand _tranferMoneyCommand;
        public DelegateCommand TranferMoneyCommand => _tranferMoneyCommand ??
                                                     (_tranferMoneyCommand = new DelegateCommand(TranferMoneyExecute));

        private DelegateCommand _autofillCommand;
        public DelegateCommand AutofillCommand => _autofillCommand ??
                                                      (_autofillCommand = new DelegateCommand(AutofillExecute));

       
        #endregion

        #region Command Handlers

        private void TranferMoneyExecute()
        {
            if (MessageBox.Show($"Вы действительно хотите зачислить {Amount} {Deposit.CurrencyType}?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (double.TryParse(Amount, out var amount))
                {
                    try
                    {
                        if (amount <= 0)
                        {
                            MessageBox.Show("Сумма для зачисления не может быть меньше или равна нулю", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        DepositManager.Instance.TransferToCurrentAccount(Deposit, amount, _serialNumber, _fio);
                        MessageBox.Show($"Средства в размере {Amount} {Deposit.CurrencyType} зачислены на {Deposit.Number}", "Оповещение", MessageBoxButton.OK, 
                            MessageBoxImage.Information);
                        _window.Close();
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось зачислить {Amount} {Deposit.CurrencyType}: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Не удалось зачислить {Amount} {Deposit.CurrencyType}: проверьте введённое число", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void AutofillExecute()
        {
            SerialNumber = Client.Passport.SerialNumber;
            FIO = $"{Client.Passport.LastName} {Client.Passport.Name} {Client.Passport.MiddleName}";
        }

        #endregion
    }
}
