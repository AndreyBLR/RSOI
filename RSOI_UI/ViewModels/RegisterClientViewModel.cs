using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RSOI_Data.Entities;
using RSOI_UI.Commands;

using RegisterClientWindow = RSOI_UI.Windows.RegisterClientWindow;

namespace RSOI_UI.ViewModels
{
    public class RegisterClientViewModel:BaseViewModel
    {
        private string _report;
        private RegisterClientWindow _window;

        #region Presentation Properties

        public string Report
        {
            get => _report;
            set
            {
                _report = value;
                OnPropertyChanged();
            }
        }

        public Client NewClient { get; set; }

        public IList<City> Cities => City.GetListOfCities();

        public IList<Disability> Disabilities => Disability.GetListOfDisabilities();

        public IList<Nationality> Nationalities => Nationality.GetListOfNationalities();

        public IList<FamilyStatus> FamilyStatuses => FamilyStatus.GetListOfFamilyStatuses();

        #endregion
        
        public RegisterClientViewModel(RegisterClientWindow window)
        {
            _window = window;
            NewClient = new Client();
        }
        
        #region Commands

        private DelegateCommand _registerClientCommand;
        public DelegateCommand RegisterClientCommand => _registerClientCommand ??
                                                      (_registerClientCommand = new DelegateCommand(RegisterClientExecute));

        #endregion

        #region Command Handlers
        
        private void RegisterClientExecute()
        {
            Report = NewClient.Verify();

            if (string.IsNullOrEmpty(Report))
            {
                try
                {
                    NewClient.Insert();
                    MessageBox.Show("Клиент зарегистрирован", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                    _window.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось зарегистрировать клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Не удалось зарегистрировать клиента, см. поле 'Отчёт'", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
