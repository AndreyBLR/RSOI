using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using RSOI_Data;
using RSOI_Data.Entities;
using RSOI_Data.Enums;
using RSOI_UI.Commands;
using RSOI_UI.Windows;
using DepositWindow = RSOI_UI.Windows.DepositWindow;
using TransferMoneyWindow = RSOI_UI.Windows.TransferMoneyWindow;

namespace RSOI_UI.ViewModels
{
    public class ContractsViewModel: BaseViewModel
    {
        private Client _client;
        private ObservableCollection<Contract> _contracts;
        private Contract _selectedContact;

        #region Presentation Properties

        public string Title { get; set; }

        public Contract SelectedContract
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Contract> Contracts
        {
            get => _contracts;
            set
            {
                _contracts = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ContractsViewModel(Client client)
        {
            _client = client;
            Contracts = new ObservableCollection<Contract>(Contract.GetListOfContractsByClient(_client.Passport.PassportId));
            Title = $"Список Договоров: {_client.Passport.Name} {_client.Passport.LastName}";
        }

        #region Commands

        private DelegateCommand _transferMoneyCommand;
        public DelegateCommand TransferMoneyCommand => _transferMoneyCommand ??
                                                                (_transferMoneyCommand = new DelegateCommand(TransferMoneyExecute, TransferMoneyCanExecute));

        private DelegateCommand _withdrawMoneyCommand;
        public DelegateCommand WithdrawMoneyCommand => _withdrawMoneyCommand ??
                                                                (_withdrawMoneyCommand = new DelegateCommand(WithdrawMoneyExecute, WithdrawMoneyCanExecute));
        
        private DelegateCommand _closeContractCommand;
        public DelegateCommand CloseContractCommand => _closeContractCommand ??
                                                      (_closeContractCommand = new DelegateCommand(CloseContractExecute, CloseContractCanExecute));
        
        private DelegateCommand _showDepositViewCommand;
        public DelegateCommand ShowDepositViewCommand => _showDepositViewCommand ??
                                                         (_showDepositViewCommand = new DelegateCommand(ShowDepositViewExecute, DataGridContextMenuCommandCanExecute));

        #endregion

        #region Command Handlers
        
        private void WithdrawMoneyExecute()
        {
            if (SelectedContract != null && SelectedContract.ContractType.Id == (int)ContractTypeIds.Deposit)
            {
                var withdrawalWindow = new WithdrawMoneyWindow(_client, Deposit.GetById(SelectedContract.Number));
                withdrawalWindow.ShowDialog();
            }
        }

        private void TransferMoneyExecute()
        {
            if (SelectedContract != null && SelectedContract.ContractType.Id == (int)ContractTypeIds.Deposit)
            {
                var transferMoneyToDepositWindow = new TransferMoneyWindow(_client, Deposit.GetById(SelectedContract.Number));
                transferMoneyToDepositWindow.ShowDialog();
            }
        }

        private void ShowDepositViewExecute()
        {
            if (SelectedContract != null && SelectedContract.ContractType.Id == (int)ContractTypeIds.Deposit)
            {
                var depositView = new DepositWindow(Deposit.GetById(SelectedContract.Number));
                depositView.ShowDialog();
            }
        }

        private void CloseContractExecute()
        {
            if (SelectedContract != null)
            {
                var deposit = Deposit.GetById(SelectedContract.Number);

                if (DateTime.Now < deposit.CloseDate)
                {
                    if (SelectedContract.EarlyClosing)
                    {
                        if (MessageBox.Show($"Срок действия договора заканчивается {deposit.CloseDate:d}. Вы действительно хотите закрыть договор досрочно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            DepositManager.Instance.CloseDeposit(Deposit.GetById(SelectedContract.Number), true);
                            MessageBox.Show("Договор закрыт досрочно.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                            Contracts = new ObservableCollection<Contract>(Contract.GetListOfContractsByClient(_client.Passport.PassportId));
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("Вы действительно хотите закрыть договор?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        DepositManager.Instance.CloseDeposit(Deposit.GetById(SelectedContract.Number), false);
                        MessageBox.Show("Договор закрыт.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                        Contracts = new ObservableCollection<Contract>(Contract.GetListOfContractsByClient(_client.Passport.PassportId));
                    }
                }
            }
        }

        private bool WithdrawMoneyCanExecute()
        {
            return DataGridContextMenuCommandCanExecute() && !SelectedContract.IsClosed;
        }

        private bool TransferMoneyCanExecute()
        {
            return DataGridContextMenuCommandCanExecute() && !SelectedContract.IsClosed;
        }

        private bool CloseContractCanExecute()
        {
            if (DataGridContextMenuCommandCanExecute() && !SelectedContract.IsClosed)
            {
                var deposit = Deposit.GetById(SelectedContract.Number);

                if (SelectedContract.EarlyClosing || DateTime.Now > deposit.CloseDate)
                {
                    return true;
                }
            }

            return false;
        }

        private bool DataGridContextMenuCommandCanExecute()
        {
            return SelectedContract != null && SelectedContract.ContractType.Id == (int)ContractTypeIds.Deposit;
        }

        #endregion
    }
}
