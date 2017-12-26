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
        public double Rate { get; set; }
        public int TermInMonth { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public bool WithWithdraw { get; set; }

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
                        Rate = (double) reader["Rate"],
                        TermInMonth = (int)reader["TermInMonth"],
                        WithWithdraw = (bool)reader["WithWithdraw"],
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int)reader["CurrencyTypeId"])
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
                
                OdbcDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    depositTypes.Add(new DepositType()
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"],
                        TermInMonth = (int) reader["TermInMonth"],
                        Rate = (double)reader["Rate"],
                        WithWithdraw = (bool)reader["WithWithdraw"],
                        CurrencyType = CurrencyType.GetCurrencyTypeById((int)reader["CurrencyTypeId"])
                    });
                }

                reader.Close();
            }

            return depositTypes;
        }

        public override string ToString()
        {
            return $" {Name} ({CurrencyType} {TermInMonth}M {Rate}%)";
        }
    }
}
