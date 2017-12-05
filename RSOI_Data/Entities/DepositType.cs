using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class DepositType : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Rate { get; set; }
        public int TermInMonth { get; set; }

        public static DepositType GetDepositTypeById(int id)
        {
            string queryString = "SELECT * FROM DepositTypes WHERE Id = '" + id + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new DepositType
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        Rate = (float) reader["Rate"],
                        TermInMonth = (int)reader["TermInMonth"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<DepositType> GetListOfDepositTypes()
        {
            var depositTypes = new List<DepositType>();

            string queryString = "SELECT * FROM DepositTypes";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    depositTypes.Add(new DepositType()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                // Call Close when done reading.
                reader.Close();
            }

            return depositTypes;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
