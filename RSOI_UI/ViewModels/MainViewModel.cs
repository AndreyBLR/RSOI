using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using RSOI_Data;
using RSOI_Data.Entities;
using RSOI_UI.Commands;

using ContractsWindow = RSOI_UI.Windows.ContractsWindow;
using OpenDepositWindow = RSOI_UI.Windows.OpenDepositWindow;
using RegisterClientWindow = RSOI_UI.Windows.RegisterClientWindow;

namespace RSOI_UI.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        private string _filter;
        private Client _selectedClient;
        private IList<Client> _clients;
        private IList<Client> _filteredClients;
        
        public MainViewModel()
        {
            _clients = Client.GetListOfClients();
            FilteredClients = new List<Client>(_clients);
        }

        #region Presentation Properties

        public IList<Client> FilteredClients
        {
            get => _filteredClients;
            set
            {
                _filteredClients = value;
                OnPropertyChanged();
            }
        }

        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                FilteredClients = new ObservableCollection<Client>(_clients.Where(item => item.Passport.SerialNumber.StartsWith(_filter)));
                OnPropertyChanged();
            }
        }

        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                _selectedClient = value;
                OnPropertyChanged();
            }
        }

        #endregion
        
        #region Commands

        private DelegateCommand _registerClientCommand;
        public DelegateCommand RegisterClientCommand => _registerClientCommand ??
                                                      (_registerClientCommand = new DelegateCommand(RegisterClientExecute));

        private DelegateCommand _closeApplicationCommand;
        public DelegateCommand CloseApplicationCommand => _closeApplicationCommand ??
                                                             (_closeApplicationCommand = new DelegateCommand(CloseApplicationExecute));
        
        private DelegateCommand _closeOperationalDayCommand;
        public DelegateCommand CloseOperationalDayCommand => _closeOperationalDayCommand ??
                                                           (_closeOperationalDayCommand = new DelegateCommand(CloseOperationalDayExecute));

        private DelegateCommand _openDepositCommand;
        public DelegateCommand OpenDepositCommand => _openDepositCommand ??
                                                     (_openDepositCommand = new DelegateCommand(OpenDepositExecute, DataGridContextMenuCommandCanExecute));

        
        private DelegateCommand _showContractsCommand;
        public DelegateCommand ShowContractsCommand => _showContractsCommand ??
                                                            (_showContractsCommand = new DelegateCommand(ShowContractsExecute, DataGridContextMenuCommandCanExecute));

        #endregion

        #region Command Handlers

        private void RegisterClientExecute()
        {
            var registerClientWindow = new RegisterClientWindow();
            registerClientWindow.ShowDialog();
        }
        
        private void CloseOperationalDayExecute()
        {
            if (MessageBox.Show("Вы действительно хотите закрыть банковский день?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    var contracts = Contract.GetListOfContracts();

                    foreach (var contract in contracts)
                    {
                        var deposit = Deposit.GetById(contract.Number);

                        if (!contract.IsClosed && DateTime.Now.Date < deposit.CloseDate.Date)
                        {
                            var income = DepositManager.CalculateDepositIncomeByDailyRate(deposit);
                            DepositManager.Instance.TransferToInterestAccounts(deposit, income);
                        }
                    }

                    MessageBox.Show("Банковский день закрыт", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось закрыть банковский день {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CloseApplicationExecute()
        {
            Application.Current.Shutdown();
        }

        private void OpenDepositExecute()
        {
            if (SelectedClient != null)
            {
                var openDepositWindow = new OpenDepositWindow(SelectedClient);
                openDepositWindow.ShowDialog();
            }
        }

        private void ShowContractsExecute()
        {
            if (SelectedClient != null)
            {
                var listOfDepositsWindow = new ContractsWindow(SelectedClient);
                listOfDepositsWindow.ShowDialog();
            }
        }

        private bool DataGridContextMenuCommandCanExecute()
        {
            return SelectedClient?.Passport.PassportId != null;
        }

        #endregion
    }
}
