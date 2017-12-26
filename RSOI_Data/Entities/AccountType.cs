using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class AccountType : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static AccountType GetAccountTypeById(int id)
        {
            string queryString = "SELECT * FROM AccountTypes WHERE Id = '" + id + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new AccountType
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<AccountType> GetListOfAccountTypes()
        {
            var accountTypes = new List<AccountType>();

            string queryString = "SELECT * FROM AccountTypes";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    accountTypes.Add(new AccountType()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Description = (string)reader["Description"]
                    });
                }
                
                reader.Close();
            }

            return accountTypes;
        }

        public string GetId()
        {
            return Id.ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
