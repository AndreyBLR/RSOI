using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using RSOI_Data.Entities;
using RSOI_UI.Views;

namespace RSOI_UI.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        private string _filter;
        private MainWindow _view;
        private IList<Client> _clients;
        private ObservableCollection<Client> _observabeClients;
        
        public MainViewModel(MainWindow view)
        {
            _view = view;
            _clients = Client.GetListOfClients();
            
            ObservableClients = new ObservableCollection<Client>(_clients);
        }

        #region Presentation Properties

        public ObservableCollection<Client> ObservableClients
        {
            get => _observabeClients;
            set
            {
                _observabeClients = value;
                OnPropertyChanged();
            }
        }

        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                OnPropertyChanged();
                ObservableClients = new ObservableCollection<Client>(_clients.Where(item => item.Passport.SerialNumber.StartsWith(_filter)));
            }
        }

        public Client SelectedClient { get; set; }

        #endregion
        
        #region Commands

        private DelegateCommand _openNewClientWindowCommand;
        public DelegateCommand OpenNewClientWindowCommand => _openNewClientWindowCommand ??
                                                      (_openNewClientWindowCommand = new DelegateCommand(OpenNewClientWindowExecute));
        
        private DelegateCommand _closeOperationalDayCommand;
        public DelegateCommand CloseOperationalDayCommand => _closeOperationalDayCommand ??
                                                           (_closeOperationalDayCommand = new DelegateCommand(CloseOperationalDayExecute));

        private DelegateCommand _openDepositCommand;
        public DelegateCommand OpenDepositCommand => _openDepositCommand ??
                                                     (_openDepositCommand = new DelegateCommand(OpenDepositExecute));

        private DelegateCommand _openListOfDepositsCommand;
        public DelegateCommand OpenListOfDepositsCommand => _openListOfDepositsCommand ??
                                                            (_openListOfDepositsCommand = new DelegateCommand(OpenListOfDepositsExecute));

        #endregion

        #region Command Handlers

        private void OpenNewClientWindowExecute()
        {
            var newClientWindow = new RegisterNewClient()
            {
                DataContext = new RegisterNewClientViewModel()
            };

            newClientWindow.ShowDialog();
        }
        
        private void CloseOperationalDayExecute()
        {
            var deposits = Deposit.GetListOfDeposits();

            foreach (var deposit in deposits)
            {
                var income = DepositManager.CalculateDepositIncomeByDailyRate(deposit);
                DepositManager.Instance.AddToInterestAccounts(deposit, income);
            }
        }

        private void OpenDepositExecute()
        {
            if (SelectedClient != null)
            {
                var openDepositWindow = new OpenDeposit();
                openDepositWindow.DataContext = new OpenDepositViewModel(openDepositWindow, SelectedClient);
                openDepositWindow.ShowDialog();
            }
        }

        private void OpenListOfDepositsExecute()
        {
            if (SelectedClient != null)
            {
                var listOfDepositsWindow = new ListOfContracts()
                {
                    DataContext = new ListOfContractsViewModel(SelectedClient)
                };

                listOfDepositsWindow.ShowDialog();
            }
        }

        #endregion
    }
}
