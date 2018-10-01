using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;

using Model;
using Service.Interface;
using System.Threading.Tasks;
using System;


namespace ToDoAPI.Controllers
{
    /// <summary>
    /// Doing the things that need to be done
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        #region Fields

        private readonly IToDoService _service;
        private readonly ILogger _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// to constructor for injection
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        public ToDoController(IToDoService service, ILogger<ToDoController> logger) => (_service, _logger) = (service, logger);
        #endregion


        // GET: api/ToDo
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/ToDo/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }


        /// <summary>
        /// creates a new list
        /// </summary>
        /// <remarks>Adds a list to the system</remarks>
        /// <param name="todoList">ToDo list to add</param>
        /// <response code="201">item created</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="409">an existing item already exists</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CreateDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create([FromBody] CreateDTO todoList)
        {
            try
            {
                if (todoList == null || !ModelState.IsValid)
                {
                    return BadRequest();
                }

                var Id = await _service.CreateToDo(todoList);
                return CreatedAtRoute("Get", new { id = Id }, todoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating ToDo");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        // PUT: api/ToDo/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
