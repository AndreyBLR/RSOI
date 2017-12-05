using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class City : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static City GetCityById(int id)
        {
            string queryString = "SELECT * FROM Cities WHERE Id = '" + id + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new City
                    {
                        Id = (int) reader["Id"],
                        Name = (string) reader["Name"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<City> GetListOfCities()
        {
            var cities = new List<City>();

            string queryString = "SELECT * FROM Cities";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
                OdbcDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    cities.Add(new City()
                    {
                        Id = (int) reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                reader.Close();
            }

            return cities;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
