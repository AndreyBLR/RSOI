using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using RSOI_Data;
using RSOI_Data.Entities;
using RSOI_UI.Commands;

using OpenDepositWindow = RSOI_UI.Windows.OpenDepositWindow;
using TransferMoneyWindow = RSOI_UI.Windows.TransferMoneyWindow;

namespace RSOI_UI.ViewModels
{
    public class OpenDepositViewModel:BaseViewModel
    {
        private Client _client;
        private DepositType _selectedDepositType;
        private OpenDepositWindow _openDepositWindow;
        private IList<DepositType> _depositTypeList;

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

        public IList<DepositType> DepositTypes
        {
            get => _depositTypeList;
            set
            {
                _depositTypeList = value;
                OnPropertyChanged();
            }
        }

        public DepositType SelectedDepositType
        {
            get => _selectedDepositType;
            set
            {
                _selectedDepositType = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public OpenDepositViewModel(OpenDepositWindow openDepositWindow, Client client)
        {
            Client = client;
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
            if (MessageBox.Show("Вы действительно хотите открыть депозит?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    var deposit = DepositManager.Instance.OpenDeposit(Client, SelectedDepositType);

                    var transferMoneyWindow = new TransferMoneyWindow(Client, deposit);
                    transferMoneyWindow.ShowDialog();
                    _openDepositWindow.Close();
                    MessageBox.Show($"Депозит открыт: {deposit.Number}", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть депозит: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}
