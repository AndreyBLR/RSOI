using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using RSOI_Data.Entities;

namespace RSOI_UI.ViewModels
{
    public class RegisterNewClientViewModel:BaseViewModel
    {
        private Client _newClient;
        
        public RegisterNewClientViewModel()
        {
            _newClient = new Client();
        }

        private string _report;

        public string Report
        {
            get => _report;
            set
            {
                _report = value;
                OnPropertyChanged(nameof(Report));
            }
        }

        #region Lists

        public IList<City> Cities => City.GetListOfCities();
        public IList<Disability> Disabilities => Disability.GetListOfDisabilities();
        public IList<Nationality> Nationalities => Nationality.GetListOfNationalities();
        public IList<FamilyStatus> FamilyStatuses => FamilyStatus.GetListOfFamilyStatuses();

        #endregion

        #region  Passport

        public string PassportId
        {
            get => _newClient.Passport.PassportId;
            set => _newClient.Passport.PassportId = value;
        }
        public string PassportNumber
        {
            get => _newClient.Passport.SerialNumber;
            set => _newClient.Passport.SerialNumber = value;
        }
        public string Name
        {
            get => _newClient.Passport.Name;
            set => _newClient.Passport.Name = value;
        }
        public string LastName
        {
            get => _newClient.Passport.LastName;
            set => _newClient.Passport.LastName = value;
        }
        public string MiddleName
        {
            get => _newClient.Passport.MiddleName;
            set => _newClient.Passport.MiddleName = value;
        }
        public bool Sex
        {
            get => _newClient.Passport.Sex;
            set => _newClient.Passport.Sex = value;
        }
        public DateTime IssueDate
        {
            get => _newClient.Passport.IssueDate;
            set => _newClient.Passport.IssueDate = value;
        }
        public string IssuedBy
        {
            get => _newClient.Passport.IssuedBy;
            set => _newClient.Passport.IssuedBy = value;
        }
        public DateTime Birthday
        {
            get => _newClient.Passport.Birthday;
            set => _newClient.Passport.Birthday = value;
        }
        public string BirthdayAddress
        {
            get => _newClient.Passport.BirthPlace;
            set => _newClient.Passport.BirthPlace = value;
        }
        public City RegCity
        {
            get => _newClient.Passport.RegCity;
            set => _newClient.Passport.RegCity = value;
        }
        public string RegAddr
        {
            get => _newClient.Passport.RegAddress;
            set => _newClient.Passport.RegAddress = value;
        }
        public Nationality Nationality
        {
            get => _newClient.Passport.Nationality;
            set => _newClient.Passport.Nationality = value;
        }
        public FamilyStatus FamilyStatus
        {
            get => _newClient.Passport.FamilyStatus;
            set => _newClient.Passport.FamilyStatus = value;
        }

        #endregion

        #region Client

        public string HomePhone
        {
            get => _newClient.HomePhone;
            set => _newClient.HomePhone = value;
        }

        public string MobilePhone
        {
            get => _newClient.MobilePhone;
            set => _newClient.MobilePhone = value;
        }

        public string Email
        {
            get => _newClient.Email;
            set => _newClient.Email = value;
        }

        public string WorkPlace
        {
            get => _newClient.WorkPlace;
            set => _newClient.WorkPlace = value;
        }

        public string WorkPosition
        {
            get => _newClient.WorkPosition;
            set => _newClient.WorkPosition = value;
        }

        public bool Reservist
        {
            get => _newClient.Reservist;
            set => _newClient.Reservist = value;
        }

        public Disability Disability
        {
            get => _newClient.Disability;
            set => _newClient.Disability = value;
        }

        public City City
        {
            get => _newClient.City;
            set => _newClient.City = value;
        }

        public string Address
        {
            get => _newClient.Address;
            set => _newClient.Address = value;
        }

        #endregion

        #region Commands

        private DelegateCommand _addNewClientCommand;

        public DelegateCommand AddNewClientCommand => _addNewClientCommand ??
                                                      (_addNewClientCommand = new DelegateCommand(AddNewClientExecute, AddNewClientCanExecute));

        #endregion

        #region Command Handlers

        private bool AddNewClientCanExecute()
        {
            return true;  //string.IsNullOrEmpty(_newClient.Verify());
        }

        private void AddNewClientExecute()
        {
            Report = _newClient.Verify();

            if (string.IsNullOrEmpty(Report))
            {
                _newClient.Insert();
            }
        }

        #endregion



    }
}
