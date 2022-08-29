using back_end.Models;
using back_end.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace back_end.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        #region Fields and Properties
        private readonly IDataService dataService;
        #endregion

        #region Constructors
        public DataController(IDataService dataService)
        {
            this.dataService = dataService;
        }
        #endregion


        #region Methods
        /// <summary>
        /// Gets data from the CSV file
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetData")]
        public ActionResult Get()
        {
            try
            {
                var data = dataService.GetData();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Saves data to the database
        /// </summary>
        /// <param name="people"></param>
        /// <returns></returns>
        [HttpPost(Name = "PostData")]
        public async Task<IActionResult> Post(List<Person> people)
        {
            try
            {
                ExecutionResult result = await dataService.SaveData(people);
                if (result.ReturnCode != 0)
                {
                    return StatusCode(400, result.ReturnMessage);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion
    }
}
