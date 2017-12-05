using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Contract : ActiveRecord
    {
        public string Number { get; set; }
        public ContractType ContractType { get; set; }
        public Client Client { get; set; }

        public static IList<Contract> GetListOfContractsByClient(int clietnId)
        {
            var contracts = new List<Contract>();

            string queryString = "SELECT * FROM Contracts WHERE ClientId = '" + clietnId + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        contracts.Add(new Contract()
                        {
                            Number = (string) reader["Number"],
                            ContractType = ContractType.GetContractTypeById((int) reader["ContractTypeId"]),
                            Client = Client.GetClientById((int) reader["ClientId"])
                        });
                    }
                }

                // Call Close when done reading.
                reader.Close();
            }

            return contracts;
        }

        public bool Save()
        {
            string queryString = "INSERT INTO Contracts " + "(" +
                                 "Number, " +
                                 "ContractTypeId, " +
                                 "ClientId)" +
                                 "VALUES ('" +
                                 Number + "','" +
                                 ContractType.Id + "','" +
                                 Client.Passport.PassportId + "')";

            try
            {
                using (OdbcConnection connection = new OdbcConnection(ConnectionString))
                {
                    connection.Open();

                    var command = new OdbcCommand(queryString, connection);

                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
