﻿using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Disability : ActiveRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Disability GetDisabilityById(int id)
        {
            string queryString = "SELECT * FROM Disabilities WHERE Id = '" + id + "'";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return new Disability()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public static IList<Disability> GetListOfDisabilities()
        {
            var disabilities = new List<Disability>();

            string queryString = "SELECT * FROM Disabilities";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    disabilities.Add(new Disability()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    });
                }
                
                reader.Close();
            }

            return disabilities;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
