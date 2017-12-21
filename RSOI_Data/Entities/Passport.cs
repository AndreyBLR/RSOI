using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Passport : ActiveRecord
    {
        public string PassportId { get; set; }
        public string SerialNumber { get; set; }
        public DateTime Birthday { get; set; }
        public bool Sex { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssueDate { get; set; }
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BirthPlace { get; set; }
        public string RegAddress { get; set; }
        public City RegCity { get; set; }
        public FamilyStatus FamilyStatus { get; set; }
        public Nationality Nationality { get; set; }

        public Passport()
        {
            IssueDate = new DateTime(1900, 1, 1);
            Birthday = new DateTime(1900, 1, 1);
        }

        public static Passport GetPassportById(string id)
        {
            string queryString = "SELECT * FROM Passports WHERE IdNumber = '" + id + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Passport()
                    {
                        PassportId = (string) reader["IdNumber"],
                        SerialNumber = (string) reader["SerialNumber"],
                        Birthday = (DateTime) reader["Birthday"],
                        Sex = (bool) reader["Sex"],
                        IssuedBy = (string) reader["IssuedBy"],
                        IssueDate = (DateTime) reader["IssueDate"],
                        Name = (string) reader["Name"],
                        MiddleName = (string) reader["MiddleName"],
                        LastName = (string) reader["LastName"],
                        BirthPlace = (string) reader["BirthPlace"],
                        RegAddress = (string) reader["RegistrationAddress"],
                        RegCity = City.GetCityById((int) reader["RegistrationCityId"]),
                        Nationality = Nationality.GetNationalityById((int) reader["NationalityId"]),
                        FamilyStatus = FamilyStatus.GetFamilyStatusById((int) reader["FamilyStatusId"])
                    };
                }

                // Call Close when done reading.
                reader.Close();
            }

            return null;
        }

        public bool Insert()
        {
            string queryString = "INSERT INTO Passports " + "(" +
                                 "IdNumber, " +
                                 "SerialNumber, " +
                                 "Birthday, " +
                                 "Sex, " +
                                 "IssuedBy, " +
                                 "IssueDate, " +
                                 "Name, " +
                                 "LastName, " +
                                 "MiddleName, " +
                                 "BirthPlace, " +
                                 "RegistrationAddress, " +
                                 "RegistrationCityId, " +
                                 "FamilyStatusId, " +
                                 "NationalityId) " +
                                 "VALUES ('" +
                                 PassportId + "','" +
                                 SerialNumber + "','" +
                                 Birthday + "','" +
                                 Sex + "','" +
                                 IssuedBy + "','" +
                                 IssueDate + "','" +
                                 Name + "','" +
                                 LastName + "','" +
                                 MiddleName + "','" +
                                 BirthPlace + "','" +
                                 RegAddress + "','" +
                                 RegCity.Id + "','" +
                                 FamilyStatus.Id + "','" +
                                 Nationality.Id + "')";


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

        public bool Delete()
        {
            string queryString = "DELETE FROM Passports WHERE IdNumber = '" + PassportId + "' ";


            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();
            }

            return true;


        }

        public string Verify()
        {
            var report = string.Empty;

            report = AddToReport(report, VerifyPassportId());
            report = AddToReport(report, VerifySerialNumber());
            report = AddToReport(report, VerifyBirthPlace());
            report = AddToReport(report, VerifyBirthday());
            report = AddToReport(report, VerifyIssueDate());
            report = AddToReport(report, VerifyIssuedBy());
            report = AddToReport(report, VerifyName());
            report = AddToReport(report, VerifyLastName());
            report = AddToReport(report, VerifyMiddleName());
            report = AddToReport(report, VerifyLastName());
            report = AddToReport(report, VerifyRegAddress());

            return report;
        }

        public override string ToString()
        {
            return PassportId;
        }

        #region Verification

        private string VerifyPassportId()
        {
            var regex = new Regex("\\d{7}\\w{1}\\d{3}\\w{2}\\d{1}", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(PassportId))
            {
                return "Пустой идентификационный номер паспорта.";
            }

            if (regex.IsMatch(PassportId))
            {
                return "";
            }

            return "Проверьте формат идентификационного номера паспорта.";
        }

        private string VerifySerialNumber()
        {
            var regex = new Regex("\\w{2}\\d{7}", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(SerialNumber))
            {
                return "Пустой серийный номер паспорта.";
            }

            if (regex.IsMatch(SerialNumber))
            {
                return "";
            }

            return "Проверьте формат серийного номера паспорта.";
        }

        private string VerifyBirthday()
        {
            if (Birthday > DateTime.Now || Birthday < new DateTime(1900, 1, 1))
            {
                return "Проверте дату рождения.";
            }

            return "";
        }

        private string VerifyIssuedBy()
        {
            if (string.IsNullOrEmpty(IssuedBy))
            {
                return "Пустое поле 'Кем Выдан'.";
            }

            return "";
        }

        private string VerifyIssueDate()
        {
            if (Birthday > DateTime.Now || Birthday < new DateTime(1900, 1, 1))
            {
                return "Проверте дату выдачи паспорта.";
            }

            return "";
        }

        private string VerifyName()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "Пустое поле 'Имя'.";
            }

            return "";
        }

        private string VerifyMiddleName()
        {
            if (string.IsNullOrEmpty(MiddleName))
            {
                return "Пустое поле 'Отчество'.";
            }

            return "";
        }

        private string VerifyLastName()
        {
            if (string.IsNullOrEmpty(LastName))
            {
                return "Пустое поле 'Фамилия'.";
            }

            return "";
        }

        private string VerifyBirthPlace()
        {
            if (string.IsNullOrEmpty(BirthPlace))
            {
                return "Пустое поле 'Место Рождения'.";
            }

            return "";
        }

        private string VerifyRegAddress()
        {
            if (string.IsNullOrEmpty(RegAddress))
            {
                return "Пустое поле 'Адрес Регистрации'.";
            }

            return "";
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
