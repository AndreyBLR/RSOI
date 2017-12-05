using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class OperationType : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static IList<OperationType> GetListOfOperationTypes()
        {
            var operationTypes = new List<OperationType>();

            string queryString = "SELECT * FROM OperationTypes";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    operationTypes.Add(new OperationType()
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

        public override string ToString()
        {
            return Name;
        }
    }
}
