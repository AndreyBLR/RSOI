using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class TransactionType : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static IList<TransactionType> GetListOfTransactionTypes()
        {
            var operationTypes = new List<TransactionType>();

            string queryString = "SELECT * FROM TransactionTypes";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    operationTypes.Add(new TransactionType()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                // Call Close when done reading.
                reader.Close();
            }

            return operationTypes;
        }

        public static TransactionType GetOperationTypeById(int operationType)
        {
            string queryString = "SELECT * FROM TransactionTypes WHERE Id = " + operationType;

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    return new TransactionType()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                }

                // Call Close when done reading.
                reader.Close();
            }

            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
