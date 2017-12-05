using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Account : ActiveRecord
    {
        public int Id { get; set; }
        public AccountType AccountType { get; set; }
        public string Number { get; set; }
        public long Amount { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public bool IsActive { get; set; }

        public static Account GetAccountById(int accountId)
        {
            string queryString = "SELECT * FROM Accounts WHERE Id = '" + accountId + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Account()
                    {
                        Id = (int)reader["Id"],
                        AccountType = AccountType.GetAccountTypeById((int)reader["AccountTypeId"]),
                        Number = (string)reader["Number"],
                        Amount = (long)reader["Amount"],
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int)reader["CurrencyTypeId"]),
                        IsActive = (bool)reader["Email"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public bool Save()
        {
            string queryString = "INSERT INTO Accounts " + "(" +
                                     "AccountTypeId, " +
                                     "Number, " +
                                     "Amount, " +
                                     "CurrencyTypeId, " +
                                     "IsActive)" +
                                 "VALUES ('" +
                                     AccountType.Id + "','" +
                                     Number + "','" +
                                     Amount + "','" +
                                     CurrencyType.Id + "','" +
                                     IsActive + "')";

            try
            {
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
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string Verify()
        {
            var report = string.Empty;

            AddToReport(report, VerifyAmount());

            return report;
        }
        
        #region Verification

        private string VerifyAmount()
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
