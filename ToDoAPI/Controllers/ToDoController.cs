using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Interface;
using Model;
using Service.Interface;

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
        private readonly IETagCache _cache;
        #endregion

        #region Constructor
        /// <summary>
        /// to constructor for injection
        /// </summary>
        /// <param name="service"></param>
        /// <param name="logger"></param>
        /// <param name="cache"></param>
        public ToDoController(IToDoService service, ILogger<ToDoController> logger, IETagCache cache) => (_service, _logger, _cache) = (service, logger, cache);
        #endregion



        /// <summary>
        /// returns all of the available lists
        /// </summary>
        /// <remarks>Searches the todo lists that are available</remarks>      
        /// <param name="searchString">pass an optional search string for looking up a list</param>
        /// <param name="skip">number of records to skip for pagination</param>
        /// <param name="limit">maximum number of records to return</param>
        /// <response code="200">search results matching criteria</response>
        /// <response code="400">bad input parameter</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ToDoDTO))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get([FromQuery]string searchString, [FromQuery]int skip, [FromQuery]int limit)
        {
            try
            {
                var skipParam = skip.Equals(0) ? 1 : skip;
                var limitParam = limit.Equals(0) ? 50 : limit;
                var toDoList = await _service.GetAllList(searchString, skipParam, limitParam);
                return Ok(toDoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ToDo list");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }                
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
                    return BadRequest();                

                var Id = await _service.CreateToDo(todoList);

                if (Id.Equals(0))
                    return StatusCode(StatusCodes.Status409Conflict);

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
