using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class PayerInfo : ActiveRecord
    {
        public int Id { get; set; }
        public string PassportSNumber { get; set; }
        public string FIO { get; set; }
        public string AdditionalInfo { get; set; }

        public static PayerInfo GetPayerInfoById(int payerInfoId)
        {
            string queryString = "SELECT * FROM PayerInfos WHERE Id = " + payerInfoId;

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    return new PayerInfo()
                    {
                        Id = (int) reader["Id"],
                        PassportSNumber = (string) reader["PassportSNumber"],
                        FIO = (string) reader["FIO"],
                        AdditionalInfo = (string) reader["AdditionalInfo"]
                    };
                }

                reader.Close();
            }

            return null;
        }

        public bool Insert()
        {
            string queryString = "INSERT INTO PayerInfos " + "(" +
                                 "PassportSNumber, " +
                                 "FIO, " +
                                 "AdditionalInfo) " +
                                 "VALUES ('" +
                                 PassportSNumber + "','" +
                                 FIO + "','" +
                                 AdditionalInfo + "')";


            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();
            }

            Id = GetLastId();
            return true;

        }

        public int GetLastId()
        {
            string queryString = "SELECT max(Id) FROM PayerInfos";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    return (int) reader[""];

                }

                reader.Close();
            }


            return -1;
        }

        public override string ToString()
        {
            return $"{FIO.Trim()} ({PassportSNumber.Trim()})";
        }
    }
}
