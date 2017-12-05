using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class FamilyStatus : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static FamilyStatus GetFamilyStatusById(int id)
        {
            string queryString = "SELECT * FROM FamilyStatuses WHERE Id = '" + id + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new FamilyStatus()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<FamilyStatus> GetListOfFamilyStatuses()
        {
            var familyStatuses = new List<FamilyStatus>();

            string queryString = "SELECT * FROM FamilyStatuses";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    familyStatuses.Add(new FamilyStatus()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }

                reader.Close();
            }

            return familyStatuses;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
