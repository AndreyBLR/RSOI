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
        public double AvailableAmount { get; set; }
        public double WithdrawAmount { get; set; }

        public static Deposit GetById(string number)
        {
            string queryString = "SELECT * FROM Deposits WHERE Number = '" + number + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
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
                        AvailableAmount = Math.Round((double) reader["AvailableAmount"], 2),
                        WithdrawAmount = Math.Round((double)reader["WithdrawAmount"], 2)
                    };
                }

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
                        AvailableAmount = Math.Round((double)reader["AvailableAmount"], 2),
                        WithdrawAmount = Math.Round((double)reader["WithdrawAmount"], 2)
                    });
                }
                
                reader.Close();
            }

            return deposits;
        }

        public void Insert()
        {
            string queryString = "INSERT INTO Deposits " + "(" +
                                 "Number, " +
                                 "OpenDate, " +
                                 "CloseDate, " +
                                 "CurrentAccountId, " +
                                 "InterestAccountId, " +
                                 "CurrencyTypeId, " +
                                 "AvailableAmount, " +
                                 "WithdrawAmount, " +
                                 "Rate)" +
                                 "VALUES ('" +
                                 Number + "','" +
                                 OpenDate + "','" +
                                 CloseDate + "','" +
                                 CurrentAccount.Id + "','" +
                                 InterestAccount.Id + "','" +
                                 CurrencyType.Id + "','" +
                                 AvailableAmount + "','" +
                                 WithdrawAmount + "','" +
                                 Rate + "')";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();
            }
        }

        public void Update()
        {
            string queryString = "UPDATE Deposits SET AvailableAmount = " + AvailableAmount + ", WithdrawAmount = " + WithdrawAmount + " WHERE Number = '" + Number + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();
            }
        }
    }
}
