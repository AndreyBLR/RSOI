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
        #region Properties

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

        #endregion

        public Passport()
        {
            Birthday = new DateTime(1970, 1, 1);
            IssueDate = new DateTime(1970, 1, 1);
        }

        public static Passport GetPassportById(string id)
        {
            string queryString = "SELECT * FROM Passports WHERE IdNumber = '" + id + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Passport
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

                reader.Close();
            }

            return null;
        }

        public void Insert()
        {
            if (string.IsNullOrEmpty(Verify()))
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


            
                using (OdbcConnection connection = new OdbcConnection(ConnectionString))
                {
                    connection.Open();

                    var command = new OdbcCommand(queryString, connection);

                    command.ExecuteNonQuery();
                }
            }
            else
            {
                throw new Exception("INSERT Passport is Failed due to verification of 'Passport' entity fields failed");
            }
        }

        public string Verify()
        {
            var report = string.Empty;

            report = AddToReport(report, VerifyPassportId());
            report = AddToReport(report, VerifySerialNumber());

            report = AddToReport(report, VerifyLastName());
            report = AddToReport(report, VerifyName());
            report = AddToReport(report, VerifyMiddleName());

            report = AddToReport(report, VerifyIssueDate());
            report = AddToReport(report, VerifyIssuedBy());

            report = AddToReport(report, VerifyBirthday());
            report = AddToReport(report, VerifyBirthPlace());
            
            report = AddToReport(report, VerifyRegCity());
            report = AddToReport(report, VerifyRegAddress());

            report = AddToReport(report, VerifyNationality());
            report = AddToReport(report, VerifyFamilyStatus());

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
                return "Пустое поле 'Идентификационный Номер Паспорта'";
            }

            if (regex.IsMatch(PassportId))
            {
                return "";
            }

            return "Проверьте формат идентификационного номера паспорта";
        }

        private string VerifySerialNumber()
        {
            var regex = new Regex("\\w{2}\\d{7}", RegexOptions.IgnoreCase);

            if (string.IsNullOrEmpty(SerialNumber))
            {
                return "Пустое поле 'Серийный Номер Паспорта'";
            }

            if (regex.IsMatch(SerialNumber))
            {
                return "";
            }

            return "Проверьте формат серийного номера паспорта";
        }

        private string VerifyBirthday()
        {
            if (Birthday > DateTime.Now || Birthday < new DateTime(1900, 1, 1))
            {
                return "Проверте поле 'Дата Рождения'";
            }

            return "";
        }

        private string VerifyIssuedBy()
        {
            if (string.IsNullOrEmpty(IssuedBy))
            {
                return "Пустое поле 'Кем Выдан'";
            }

            return "";
        }

        private string VerifyIssueDate()
        {
            if (IssueDate > Birthday)
            {
                return "Проверте поле 'Дата Выдачи Паспорта'";
            }

            return "";
        }

        private string VerifyName()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "Пустое поле 'Имя'";
            }

            return "";
        }

        private string VerifyMiddleName()
        {
            if (string.IsNullOrEmpty(MiddleName))
            {
                return "Пустое поле 'Отчество'";
            }

            return "";
        }

        private string VerifyLastName()
        {
            if (string.IsNullOrEmpty(LastName))
            {
                return "Пустое поле 'Фамилия'";
            }

            return "";
        }

        private string VerifyBirthPlace()
        {
            if (string.IsNullOrEmpty(BirthPlace))
            {
                return "Пустое поле 'Место Рождения'";
            }

            return "";
        }

        private string VerifyRegCity()
        {
            if (RegCity == null)
            {
                return "Пустое поле 'Город Прописки'";
            }

            return "";
        }

        private string VerifyRegAddress()
        {
            if (string.IsNullOrEmpty(RegAddress))
            {
                return "Пустое поле 'Адрес Прописки'";
            }

            return "";
        }
        
        private string VerifyNationality()
        {
            if (RegCity == null)
            {
                return "Пустое поле 'Национальность'";
            }

            return "";
        }

        private string VerifyFamilyStatus()
        {
            if (RegCity == null)
            {
                return "Пустое поле 'Семейный Статус'";
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
