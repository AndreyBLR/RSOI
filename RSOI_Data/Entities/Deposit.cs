using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Deposit : ActiveRecord
    {
        public string Number { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public double Rate { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public Account CurrentAccount { get; set; }
        public Account InterestAccount { get; set; }
        public double Amount { get; set; }

        public static Deposit GetById(string number)
        {
            string queryString = "SELECT * FROM Deposits WHERE Number = '" + number + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Deposit()
                    {
                        Number = (string)reader["Number"],
                        OpenDate = (DateTime)reader["OpenDate"],
                        CloseDate = (DateTime)reader["CloseDate"],
                        Rate = (double)reader["Rate"],
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int)reader["CurrencyTypeId"]),
                        CurrentAccount = Account.GetAccountById((int)reader["CurrentAccountId"]),
                        InterestAccount = Account.GetAccountById((int)reader["InterestAccountId"]),
                        Amount = Math.Round((double) reader["Amount"], 2)
                    };
                }

                // Call Close when done reading.
                reader.Close();
            }

            return null;
        }

        public static IList<Deposit> GetListOfDeposits()
        {
            var deposits = new List<Deposit>();
            string queryString = "SELECT * FROM Deposits";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    deposits.Add(new Deposit()
                    {
                        Number = (string)reader["Number"],
                        OpenDate = (DateTime)reader["OpenDate"],
                        CloseDate = (DateTime)reader["CloseDate"],
                        Rate = (double)reader["Rate"],
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int)reader["CurrencyTypeId"]),
                        CurrentAccount = Account.GetAccountById((int)reader["CurrentAccountId"]),
                        InterestAccount = Account.GetAccountById((int)reader["InterestAccountId"]),
                        Amount = (double)reader["Amount"]
                    });
                }

                // Call Close when done reading.
                reader.Close();
            }

            return deposits;
        }

        public bool Insert()
        {
            string queryString = "INSERT INTO Deposits " + "(" +
                                 "Number, " +
                                 "OpenDate, " +
                                 "CloseDate, " +
                                 "CurrentAccountId, " +
                                 "InterestAccountId, " +
                                 "CurrencyTypeId, " +
                                 "Amount, " +
                                 "Rate)" +
                                 "VALUES ('" +
                                 Number + "','" +
                                 OpenDate + "','" +
                                 CloseDate + "','" +
                                 CurrentAccount.Id + "','" +
                                 InterestAccount.Id + "','" +
                                 CurrencyType.Id + "','" +
                                 Amount + "','" +
                                 Rate + "')";


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

        public bool Update()
        {
            string queryString = "UPDATE Deposits SET Amount = " + Amount + " WHERE Number = '" + Number + "'";


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


        public string Verify()
        {
            var report = string.Empty;

            AddToReport(report, VerifyRate());
            AddToReport(report, VerifyDates());

            return report;
        }

        #region Verification

        private string VerifyRate()
        {
            return "";
        }

        private string VerifyDates()
        {
            return "";
        }

        private string AddToReport(string report, string error)
        {
            if (!string.IsNullOrEmpty(error))
                return report + error + "; \n";

            return report;
        }

        #endregion
    }
}
