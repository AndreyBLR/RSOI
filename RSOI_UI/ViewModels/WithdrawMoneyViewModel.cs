using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RSOI_Data;
using RSOI_Data.Entities;
using RSOI_UI.Commands;

using RSOI_UI.Windows;

namespace RSOI_UI.ViewModels
{
    public class WithdrawMoneyViewModel : BaseViewModel
    {
        private string _amount;
        private Client _client;
        private Deposit _deposit;
        private WithdrawMoneyWindow _window;

        #region Presentation Properties

        public string Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

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

        #endregion

        public WithdrawMoneyViewModel(WithdrawMoneyWindow window, Client client, Deposit deposit)
        {
            Client = client;
            Deposit = deposit;
            _window = window;
        }

        #region Commands

        private DelegateCommand _withdrawMoneyCommand;
        public DelegateCommand WithdrawMoneyCommand => _withdrawMoneyCommand ??
                                                     (_withdrawMoneyCommand = new DelegateCommand(WithdrawMoneyExecute));

        #endregion

        #region Command Handlers

        private void WithdrawMoneyExecute()
        {
            if (MessageBox.Show($"Вы действительно хотите списать {Amount} {Deposit.CurrencyType}?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (double.TryParse(Amount, out var amount))
                {
                    if (amount > Deposit.InterestAccount.Amount)
                    {
                        MessageBox.Show("Введённая сумма превышает доступное количество для списания", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (amount <= 0)
                    {
                        MessageBox.Show("Сумма для списания не может быть меньше или равна нулю", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    try
                    {
                        DepositManager.Instance.WithdrawFromInterestAccount(Deposit, amount);
                        MessageBox.Show("Средства списаны", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                        _window.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Не удалось списать {Amount} {Deposit.CurrencyType}: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Не удалось списать {Amount} {Deposit.CurrencyType}: проверьте введённое число", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}
