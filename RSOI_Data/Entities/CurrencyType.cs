using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class CurrencyType:ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static CurrencyType GetCurrencyTypeById(int id)
        {
            string queryString = "SELECT * FROM CurrencyTypes WHERE Id = '" + id + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new CurrencyType
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<CurrencyType> GetListOfCurrencyTypes()
        {
            var currencyTypes = new List<CurrencyType>();

            string queryString = "SELECT * FROM CurrencyTypes";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    currencyTypes.Add(new CurrencyType()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                // Call Close when done reading.
                reader.Close();
            }

            return currencyTypes;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
