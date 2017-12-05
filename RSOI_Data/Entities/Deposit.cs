using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Deposit : ActiveRecord
    {
        public string Number { get; set; }
        public DepositType DepositType { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime CloseDate { get; set; }
        public int Rate { get; set; }

        public static Deposit GetById(int number)
        {
            string queryString = "SELECT * FROM Deposits WHERE Number = '" + number + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Deposit()
                    {
                        Number = (string)reader["Number"],
                        DepositType = DepositType.GetDepositTypeById((int)reader["DepositTypeId"]),
                        OpenDate = (DateTime)reader["OpenDate"],
                        CloseDate = (DateTime)reader["CloseDate"],
                        Rate = (int)reader["Rate"]
                    };
                }

                // Call Close when done reading.
                reader.Close();
            }

            return null;
        }

        public bool Save()
        {
            string queryString = "INSERT INTO Contracts " + "(" +
                                     "Number, " +
                                     "DepositTypeId, " +
                                     "OpenDate, " +
                                     "CloseDate, " +
                                     "Rate)" +
                                 "VALUES ('" +
                                     Number + "','" +
                                     DepositType.Id + "','" +
                                     OpenDate + "','" +
                                     CloseDate + "','" +
                                     Rate + "')";


            try
            {
                if (string.IsNullOrEmpty(Verify()))
                {
                    using (OdbcConnection connection = new OdbcConnection(ConnectionString))
                    {
                        connection.Open();

                        var command = new OdbcCommand(queryString, connection);

                        command.ExecuteNonQuery();
                    }
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string Verify()
        {
            var report = string.Empty;

            AddToReport(report, VerifyRate());
            AddToReport(report, VerifyDates());

            return report;
        }

        #region Verification

        private string VerifyRate()
        {
            return "";
        }

        private string VerifyDates()
        {
            return "";
        }

        private string AddToReport(string report, string error)
        {
            if (!string.IsNullOrEmpty(error))
                return report + error + "; \n";

            return report;
        }

        #endregion
    }
}
