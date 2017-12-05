using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Nationality : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Nationality GetNationalityById(int id)
        {
            string queryString = "SELECT * FROM Nationalities WHERE Id = '" + id + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new Nationality()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<Nationality> GetListOfNationalities()
        {
            var nationalities = new List<Nationality>();

            string queryString = "SELECT * FROM Nationalities";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    nationalities.Add(new Nationality()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }
                
                reader.Close();
            }

            return nationalities;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
