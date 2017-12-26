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
        public Credentials Operator { get; set; }

        public static Contract GetContractByNumber(string numberId)
        {
            string queryString = "SELECT * FROM Contracts WHERE Number = '" + numberId + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
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
                
                reader.Close();
            }

            return null;
        }

        public static IList<Contract> GetListOfContracts()
        {
            var contracts = new List<Contract>();

            string queryString = "SELECT * FROM Contracts";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();

                OdbcDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        contracts.Add(new Contract()
                        {
                            Number = (string)reader["Number"],
                            ContractType = ContractType.GetContractTypeById((int)reader["ContractTypeId"]),
                            Client = Client.GetClientById((string)reader["ClientId"]),
                            IsClosed = (bool)reader["IsClosed"],
                            EarlyClosing = (bool)reader["EarlyClosing"],
                            Operator = Credentials.GetCredentialsById((int)reader["OperatorId"])
                        });
                    }
                }

                reader.Close();
            }

            return contracts;
        }

        public static IList<Contract> GetListOfContractsByClient(string passportId)
        {
            var contracts = new List<Contract>();

            string queryString = "SELECT * FROM Contracts WHERE ClientId = '" + passportId + "' ";

            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                OdbcCommand command = new OdbcCommand(queryString, connection);

                connection.Open();
                
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
                            EarlyClosing = (bool)reader["EarlyClosing"],
                            Operator = Credentials.GetCredentialsById((int)reader["OperatorId"])
                        });
                    }
                }
                
                reader.Close();
            }

            return contracts;
        }
        
        public void Insert()
        {
            string queryString = "INSERT INTO Contracts " + "(" +
                                 "Number, " +
                                 "ContractTypeId, " +
                                 "IsClosed, " +
                                 "EarlyClosing, " +
                                 "OperatorId, " +
                                 "ClientId) " +
                                 "VALUES ('" +
                                 Number + "','" +
                                 ContractType.Id + "','" +
                                 IsClosed + "','" +
                                 EarlyClosing + "','" +
                                 Operator.Id + "','" +
                                 Client.Passport.PassportId + "')";
            
            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();

                var command = new OdbcCommand(queryString, connection);

                command.ExecuteNonQuery();
            }
        }

        public void Update()
        {
            string queryString = "UPDATE Contracts SET IsClosed = '" + IsClosed + "', OperatorId = "+ Operator.Id + " WHERE Number = '" + Number + "'";
            
            using (OdbcConnection connection = new OdbcConnection(ConnectionString))
            {
                connection.Open();
                var command = new OdbcCommand(queryString, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
