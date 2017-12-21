using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Client : ActiveRecord
    {
        public Passport Passport { get; set; }
        public City City { get; set; }
        public string Address { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string WorkPlace { get; set; }
        public string WorkPosition { get; set; }
        public long MonthIncome { get; set; }
        public bool Reservist { get; set; }
        public Disability Disability { get; set; }

        public Client()
        {
            Passport = new Passport();
        }

        public static Client GetClientById(string passportId)
        {
            string queryString = "SELECT * FROM Clients WHERE PassportId = '" + passportId + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Client()
                    {
                        Passport = Passport.GetPassportById((string) reader["PassportId"]),
                        City = City.GetCityById((int) reader["CityId"]),
                        Address = (string) reader["Address"],
                        HomePhone = (string) reader["HomePhone"],
                        MobilePhone = (string) reader["MobilePhone"],
                        Email = (string) reader["Email"],
                        WorkPlace = (string) reader["WorkPlace"],
                        WorkPosition = (string) reader["WorkPosition"],
                        MonthIncome = (long) reader["MonthIncome"],
                        Reservist = (bool) reader["Reservist"],
                        Disability = Disability.GetDisabilityById((int) reader["DisabilityId"])
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<Client> GetListOfClients()
        {
            var clients = new List<Client>();

            string queryString = "SELECT * FROM Clients";


            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clients.Add(new Client()
                    {
                        Passport = Passport.GetPassportById((string) reader["PassportId"]),
                        City = City.GetCityById((int) reader["CityId"]),
                        Address = (string) reader["Address"],
                        HomePhone = (string) reader["HomePhone"],
                        MobilePhone = (string) reader["MobilePhone"],
                        Email = (string) reader["Email"],
                        WorkPlace = (string) reader["WorkPlace"],
                        WorkPosition = (string) reader["WorkPosition"],
                        MonthIncome = (long) reader["MonthIncome"],
                        Reservist = (bool) reader["Reservist"],
                        Disability = Disability.GetDisabilityById((int) reader["DisabilityId"])
                    });
                }

                reader.Close();
            }


            return clients;
        }

        public bool Insert()
        {
            if (Passport.Insert())
            {
                string queryString = "INSERT INTO Clients " + "(" +
                                     "PassportId, " +
                                     "Address, " +
                                     "HomePhone, " +
                                     "MobilePhone, " +
                                     "Email, " +
                                     "WorkPlace, " +
                                     "WorkPosition, " +
                                     "MonthIncome, " +
                                     "Reservist, " +
                                     "DisabilityId, " +
                                     "CityId)" +
                                     "VALUES ('" +
                                     Passport.PassportId + "','" +
                                     Address + "','" +
                                     HomePhone + "','" +
                                     MobilePhone + "','" +
                                     Email + "','" +
                                     WorkPlace + "','" +
                                     WorkPosition + "','" +
                                     MonthIncome + "','" +
                                     Reservist + "','" +
                                     Disability.Id + "','" +
                                     City.Id + "')";

                if (string.IsNullOrEmpty(Verify()))
                {
                    using (OdbcConnection connection = new OdbcConnection(ConnectionString))
                    {
                        connection.Open();

                        var command = new OdbcCommand(queryString, connection);

                        command.ExecuteNonQuery();
                    }

                    return true;
                }

                return false;

            }

            return false;
        }

        public bool Delete()
        {
            string queryString = "DELETE FROM Passports WHERE IdNumber = '" + Passport.PassportId + "' ";

            try
            {
                using (OdbcConnection connection = new OdbcConnection(ConnectionString))
                {
                    connection.Open();

                    var command = new OdbcCommand(queryString, connection);

                    command.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public string Verify()
        {
            var report = Passport.Verify();

            report = AddToReport(report, VerifyAddress());
            report = AddToReport(report, VerifyEmail());
            report = AddToReport(report, VerifyMobilePhone());
            report = AddToReport(report, VerifyMonthIncome());

            return report;
        }

        public override string ToString()
        {
            return Passport.SerialNumber;
        }

        #region Verification

        private string VerifyAddress()
        {
            if (string.IsNullOrEmpty(Address))
            {
                return "Пустое поле 'Адрес Проживания'.";
            }

            return "";
        }

        private string VerifyMobilePhone()
        {
            var regex = new Regex("\\d{9}", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(MobilePhone))
            {
                return "";
            }

            if (regex.IsMatch(MobilePhone))
            {
                return "";
            }

            return
                "Номер мобильного телефона должен содержать 9 цифр (2 цифры - код оператора, 7 цифр - номер телефона).";
        }

        private string VerifyEmail()
        {
            var regex = new Regex(@"^[a-z0-9][-a-z0-9.!#$%&'*+-=?^_`{|}~\/]+@([-a-z0-9]+\.)+[a-z]{2,5}$",
                RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(Email))
            {
                return "";
            }

            if (regex.IsMatch(Email))
            {
                return "";
            }

            return "Email должен соответствовать формату name@domain.";
        }

        private string VerifyMonthIncome()
        {
            if (MonthIncome >= 0)
            {
                return "";
            }

            return "Месячный доход не может быть отрицательным значением.";
        }

        private string AddToReport(string report, string error)
        {
            if (!string.IsNullOrEmpty(error))
                return report + error + "\n";

            return report;
        }

        #endregion
    }
}
