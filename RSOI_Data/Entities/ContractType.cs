using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class ContractType : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static ContractType GetContractTypeById(int id)
        {
            string queryString = "SELECT * FROM ContractTypes WHERE Id = '" + id + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new ContractType
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<ContractType> GetListOfContractTypes()
        {
            var contractType = new List<ContractType>();

            string queryString = "SELECT * FROM ContractTypes";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    contractType.Add(new ContractType()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                // Call Close when done reading.
                reader.Close();
            }

            return contractType;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
