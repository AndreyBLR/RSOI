using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Transaction : ActiveRecord
    {
        public int Id { get; set; }
        public Account AccountId1 { get; set; }
        public Account AccountId2 { get; set; }
        public TransactionType TransactionType { get; set; }
        public double Sum { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public PayerInfo PayerInfo { get; set; }
        public Credentials Operator { get; set; }

        public void Insert()
        {
            PayerInfo?.Insert();

            string queryString = "INSERT INTO Transactions " + "(" +
                                 "AccountId1, " +
                                 "AccountId2, " +
                                 "TransactionTypeId, " +
                                 "Sum, " +
                                 "DateTime, " +
                                 "OperatorId, " +
                                 "Description" +
                                 (PayerInfo == null ? ")" : ", PayerInfoId)") +
                                 "VALUES ('" +
                                 AccountId1.Id + "','" +
                                 AccountId2.Id + "','" +
                                 TransactionType.Id + "','" +
                                 Sum + "','" +
                                 DateTime + "','" +
                                 Operator.Id + "','" +
                                 Description + "'" +
                                 (PayerInfo == null ? ")" : ", "+PayerInfo.Id + ")");


            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();

                Thread.Sleep(100);
            }
        }

        public static IList<Transaction> GetListOfTransactionsByAccountId(int accountId)
        {
            var transactions = new List<Transaction>();

            string queryString = "SELECT * FROM Transactions WHERE AccountId1 = " + accountId + " OR AccountId2 = " + accountId;

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var transaction = new Transaction()
                        {
                            Id = (int) reader["Id"],
                            AccountId1 = Account.GetAccountById((int) reader["AccountId1"]),
                            AccountId2 = Account.GetAccountById((int) reader["AccountId2"]),
                            TransactionType = TransactionType.GetOperationTypeById((int) reader["TransactionTypeId"]),
                            Sum = Math.Round((double) reader["Sum"], 2),
                            Description = (string) reader["Description"],
                            DateTime = (DateTime) reader["DateTime"],
                            Operator = Credentials.GetCredentialsById((int)reader["OperatorId"])
                        };

                        if (reader["PayerInfoId"] != DBNull.Value)
                        {
                            transaction.PayerInfo = PayerInfo.GetPayerInfoById((int)reader["PayerInfoId"]);
                        }

                        transactions.Add(transaction);
                    }
                }

                reader.Close();
            }

            return transactions;
        }
    }
}
