using back_end.Models;

namespace back_end.Services
{
    public interface IDataService
    {
        /// <summary>
        /// Gets data from CSV file
        /// </summary>
        /// <returns></returns>
        IEnumerable<Person> GetData();

        /// <summary>
        /// Saves data to the database
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ExecutionResult> SaveData(List<Person> data);
    }
}
