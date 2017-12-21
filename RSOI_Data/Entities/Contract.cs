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
        public Client Client { get; set; }
        public ContractType ContractType { get; set; }
        public bool IsClosed { get; set; }
        public bool EarlyClosing { get; set; }

        public static Contract GetContractByNumber(string numberId)
        {
            string queryString = "SELECT * FROM Contracts WHERE Number = '" + numberId + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    return new Contract()
                    {
                        Number = (string) reader["Number"],
                        ContractType = ContractType.GetContractTypeById((int) reader["ContractTypeId"]),
                        Client = Client.GetClientById((string) reader["ClientId"]),
                        IsClosed = (bool) reader["IsClosed"],
                        EarlyClosing = (bool)reader["EarlyClosing"]
                    };

                }

                // Call Close when done reading.
                reader.Close();
            }

            return null;
        }

        public static IList<Contract> GetListOfContractsByClient(string passportId)
        {
            var contracts = new List<Contract>();

            string queryString = "SELECT * FROM Contracts WHERE ClientId = '" + passportId + "' ";

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
                            Client = Client.GetClientById((string) reader["ClientId"]),
                            IsClosed = (bool)reader["IsClosed"],
                            EarlyClosing = (bool)reader["EarlyClosing"]
                        });
                    }
                }

                // Call Close when done reading.
                reader.Close();
            }

            return contracts;
        }

        public bool Insert()
        {
            string queryString = "INSERT INTO Contracts " + "(" +
                                 "Number, " +
                                 "ContractTypeId, " +
                                 "IsClosed, " +
                                 "EarlyClosing, " +
                                 "ClientId)" +
                                 "VALUES ('" +
                                 Number + "','" +
                                 ContractType.Id + "','" +
                                 IsClosed + "','" +
                                 EarlyClosing + "','" +
                                 Client.Passport.PassportId + "')";

           
                using (OdbcConnection connection = new OdbcConnection(ConnectionString))
                {
                    connection.Open();

                    var command = new OdbcCommand(queryString, connection);

                    command.ExecuteNonQuery();
                }
                return true;
           
        }

        public bool Update()
        {
            string queryString = "UPDATE Contracts SET IsClosed = '" + IsClosed + "' WHERE Number = '" + Number + "'";


            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();
            }

            return true;

        }
    }
}
