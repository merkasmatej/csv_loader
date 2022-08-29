using back_end.Models;
using System.Data;
using System.Data.SqlClient;

namespace back_end.Services
{
    public class DataService : IDataService
    {
        #region Fields and Properties

        private readonly string addPersonStoredProcedureName = "dbo.AddPerson";

        private readonly IConfiguration configuration;

        #endregion

        #region Constructors
        public DataService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets data from CSV file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Person> GetData()
        {
            List<Person> people = new List<Person>();

            string filePath = configuration["FilePath"];
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                using(var reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string? line = reader.ReadLine();
                        string[] values = line.Split(';');
                        
                        people.Add(MapStringArrayToPerson(values));
                    }
                }
            }

            return people;
        }

        /// <summary>
        /// Saves data to the database
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ExecutionResult> SaveData(List<Person> data)
        {
            List<ExecutionResult> executionResults = new List<ExecutionResult>();

            string connectionString = configuration["ConnectionString"];

            if (data != null && data.Any() && !string.IsNullOrWhiteSpace(connectionString))
            {
                foreach (var item in data)
                {
                    if (item.IsPostalCodeValid)
                    {
                        var parametes = CreateAddPersonParameters(item);
                        var storedProcedureResult = await ExecuteStoredProcedureAsync(addPersonStoredProcedureName, parametes, connectionString);
                        executionResults.Add(storedProcedureResult);
                    }
                }
            }

            return CreateExecutionResult(executionResults);
        }

        #region Helper Methods

        /// <summary>
        /// Maps array of string to Person object
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private Person MapStringArrayToPerson(string[] values)
        {
            int postalCode;

            Person person = new Person()
            {
                FirstName = values[0],
                LastName = values[1],
                PostalCode = values[2],
                City = values[3],              
                PhoneNumber = values[4]
            };

            person.IsPostalCodeValid = int.TryParse(values[2], out postalCode);

            return person;
        } 

        /// <summary>
        /// Creates parameters for stored procedure
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        private Dictionary<string, string> CreateAddPersonParameters(Person person)
        {
            return new Dictionary<string, string>()
            {
                    { "@ime", person.FirstName },
                    { "@prezime", person.LastName },
                    { "@postanski_broj", person.PostalCode },
                    { "@grad", person.City },
                    { "@telefon", person.PhoneNumber },
            };
        }

        /// <summary>
        /// Executes stored procedure
        /// </summary>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="connectionString"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<ExecutionResult> ExecuteStoredProcedureAsync(string storedProcedureName, Dictionary<string, string> parameters, string connectionString, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            })
            using (cancellationToken.Register(() => command.Cancel()))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                }
                var returnMessage = command.Parameters.Add("@returnMsg", SqlDbType.NVarChar, -1);
                returnMessage.Direction = ParameterDirection.Output;

                SqlParameter returnCodeParam = new SqlParameter();
                returnCodeParam.Direction = ParameterDirection.ReturnValue;
                command.Parameters.Add(returnCodeParam);

                await connection.OpenAsync(cancellationToken);

                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

                var result = new ExecutionResult()
                {
                    ReturnCode = (int)returnCodeParam.Value,
                    ReturnMessage = returnMessage.Value.ToString(),
                };
                return result;
            }
        }

        /// <summary>
        /// Creates Execution result object from List of Execution results
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private ExecutionResult CreateExecutionResult(List<ExecutionResult> results)
        {
            if (results.Any(r => r.ReturnCode != 0))
            {
                var messages = results.Where(r => r.ReturnCode != 0).Select(m => m.ReturnMessage).ToList();

                return new ExecutionResult()
                {
                    ReturnMessage = String.Join(". ", messages),
                    ReturnCode = results.First(r => r.ReturnCode != 0).ReturnCode
                };
            }

            return new ExecutionResult()
            {
                ReturnCode = 0,
                ReturnMessage = "Ok"
            };
        }
        #endregion

        #endregion
    }
}
