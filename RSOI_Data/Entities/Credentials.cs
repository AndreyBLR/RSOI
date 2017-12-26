using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Credentials : ActiveRecord
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FIO { get; set; }

        public static Credentials GetCredentialsById(int id)
        {
            string queryString = "SELECT FIO FROM Credentials WHERE Id = '" + id + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Credentials()
                    {
                        Id = id,
                        FIO = (string) reader["FIO"]
                    };

                }

                reader.Close();
            }

            return null;
        }

        public static Credentials GetCredentialsByLoginAndPassword(string login, string password)
        {
            string queryString = "SELECT Id, FIO FROM Credentials WHERE Login = '" + login + "' AND Password = '" + password + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Credentials()
                    {
                        Id = (int) reader["Id"],
                        FIO = (string)reader["FIO"]
                    };

                }

                reader.Close();
            }

            return null;
        }

        public override string ToString()
        {
            return FIO.Trim();
        }
    }
}
