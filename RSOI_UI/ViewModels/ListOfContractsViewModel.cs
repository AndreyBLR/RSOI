using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using RSOI_Data.Entities;
using RSOI_Data.Enums;
using RSOI_UI.Views;

namespace RSOI_UI.ViewModels
{
    public class ListOfContractsViewModel: BaseViewModel
    {
        private Client _client;
        private ObservableCollection<Contract> _contracts;
        private Contract _selectedContact;

        public ListOfContractsViewModel(Client client)
        {
            _client = client;
            ListOfContracts = new ObservableCollection<Contract>(Contract.GetListOfContractsByClient(_client.Passport.PassportId));
            Title = $"Список Контрактов: {_client.Passport.Name} {_client.Passport.LastName}";
        }

        #region Commands

        private DelegateCommand _transferMoneyToDepositCommand;
        public DelegateCommand TransferMoneyToDepositCommand => _transferMoneyToDepositCommand ??
                                                                (_transferMoneyToDepositCommand = new DelegateCommand(TransferMoneyExecute, TransferMoneyCanExecute));

        private DelegateCommand _closeContractCommand;
        public DelegateCommand CloseContractCommand => _closeContractCommand ??
                                                      (_closeContractCommand = new DelegateCommand(CloseContractExecute, CloseContractCanExecute));
        
        private DelegateCommand _showDepositViewCommand;
        public DelegateCommand ShowDepositViewCommand => _showDepositViewCommand ??
                                                         (_showDepositViewCommand = new DelegateCommand(ShowDepositViewExecute));

        #endregion

        #region Command Handlers

        private void TransferMoneyExecute()
        {
            if (SelectedContract != null && SelectedContract.ContractType.Id == (int)ContractTypeIds.Deposit)
            {
                var transferMoneyToDepositWindow = new TransferMoneyToDeposit();
                transferMoneyToDepositWindow.DataContext = new TransferMoneyToDepositViewModel(transferMoneyToDepositWindow, _client, Deposit.GetById(SelectedContract.Number));

                transferMoneyToDepositWindow.ShowDialog();
            }
        }

        private bool TransferMoneyCanExecute()
        {
            return SelectedContract != null && SelectedContract.ContractType.Id == (int)ContractTypeIds.Deposit;
        }

        private void ShowDepositViewExecute()
        {
            if (SelectedContract != null && SelectedContract.ContractType.Id == (int)ContractTypeIds.Deposit)
            {
                var depositView = new DepositView()
                {
                    DataContext = new DepositViewModel(Deposit.GetById(SelectedContract.Number))
                };

                depositView.ShowDialog();
            }
        }

        private void CloseContractExecute()
        {
            if (SelectedContract != null && SelectedContract.EarlyClosing)
            {
                var deposit = Deposit.GetById(SelectedContract.Number);

                DepositManager.Instance.ZeroyingInterestAccount(deposit, "Обнудение процентного счёта в связи с досрочным закрытием депозита.");
                DepositManager.Instance.CloseDeposit(deposit);
                DepositManager.Instance.WithdrayFromCurrentAccount(deposit, deposit.Amount);

                MessageBox.Show("Депозит Закрыт.", "Результат");
                
                ListOfContracts = new ObservableCollection<Contract>(Contract.GetListOfContractsByClient(_client.Passport.PassportId));
            }
        }

        private bool CloseContractCanExecute()
        {
            return SelectedContract != null && SelectedContract.EarlyClosing;
        }

        #endregion

        #region Presentation Properties

        public string Title { get; set; }

        public Contract SelectedContract
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged(TransferMoneyToDepositCommand);
                RaiseCanExecuteChanged(CloseContractCommand);
            }
        }

        public ObservableCollection<Contract> ListOfContracts
        {
            get => _contracts;
            set
            {
                _contracts = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
