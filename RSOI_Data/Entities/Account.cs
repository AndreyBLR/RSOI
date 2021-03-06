﻿using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSOI_Data.Enums;

namespace RSOI_Data.Entities
{
    public class Account : ActiveRecord
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public AccountType AccountType { get; set; }
        public double Amount { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public bool IsActive { get; set; }

        public static IList<Account> GetListOfAccounts()
        {
            string queryString = "SELECT * FROM Accounts";

            var accounts = new List<Account>();

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    accounts.Add(new Account()
                    {
                        Id = (int) reader["Id"],
                        AccountType = AccountType.GetAccountTypeById((int) reader["AccountTypeId"]),
                        Amount = Math.Round((double) reader["Amount"], 2),
                        Number = reader["Number"] != DBNull.Value ? (string)reader["Number"] : "",
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int) reader["CurrencyTypeId"]),
                        IsActive = (bool) reader["IsActive"]
                    });
                }

                reader.Close();
            }

            return accounts;
        }

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
                        Id = (int) reader["Id"],
                        AccountType = AccountType.GetAccountTypeById((int) reader["AccountTypeId"]),
                        Amount = Math.Round((double) reader["Amount"], 2),
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int) reader["CurrencyTypeId"]),
                        Number = reader["Number"] != DBNull.Value ? (string)reader["Number"] : "",
                        IsActive = (bool) reader["IsActive"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static Account GetAccountByType(int typeId)
        {
            string queryString = "SELECT * FROM Accounts WHERE AccountTypeId = '" + typeId + "' ";

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
                        Id = (int) reader["Id"],
                        AccountType = AccountType.GetAccountTypeById((int) reader["AccountTypeId"]),
                        Amount = Math.Round((double) reader["Amount"], 2),
                        Number = reader["Number"] != DBNull.Value ? (string)reader["Number"] : "",
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int) reader["CurrencyTypeId"]),
                        IsActive = (bool) reader["IsActive"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static Account GetAccountByTypeAndCurrency(int typeId, int currencyId)
        {
            string queryString = "SELECT * FROM Accounts WHERE AccountTypeId = " + typeId + " AND CurrencyTypeId = " + currencyId + " ";

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
                        Id = (int) reader["Id"],
                        AccountType = AccountType.GetAccountTypeById((int) reader["AccountTypeId"]),
                        Amount = Math.Round((double) reader["Amount"],2 ),
                        Number = reader["Number"] != DBNull.Value ? (string)reader["Number"] : "",
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int) reader["CurrencyTypeId"]),
                        IsActive = (bool) reader["IsActive"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public void Insert()
        {
            string queryString = "INSERT INTO Accounts " + "(" +
                                 "AccountTypeId, " +
                                 "Amount, " +
                                 "Number, " + 
                                 "CurrencyTypeId, " +
                                 "IsActive)" +
                                 "VALUES ('" +
                                 AccountType.Id + "','" +
                                 Amount + "','" +
                                 Number + "','" +
                                 CurrencyType.Id + "','" +
                                 IsActive + "')";



            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();
            }

            Id = GetLastId();
        }

        public void Update()
        {
            string queryString = "UPDATE Accounts SET Amount = " + Amount + " WHERE Id = '" + Id + "'";
            
            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();
            }
        }

        public override string ToString()
        {
            return GetIBAN();
        }

        private int GetLastId()
        {
            string queryString = "SELECT max(Id) FROM Accounts";
            
            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return (int) reader[""];
                }

                reader.Close();
            }

            return -1;
        }

        private string GetIBAN()
        {
            return $"BY131782{AccountType.GetId()}{Number}0000{CurrencyType.GetId()}";
        }
    }
}
