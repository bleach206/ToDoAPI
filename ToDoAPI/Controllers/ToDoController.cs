using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Common.Interface;
using Model;
using Service.Interface;
using Model.Interface;
using System.Linq;
using System.Net;

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
        /// <remarks>Gets a list of user to do. <br />  id: user id. <br />  skip: number of records to skip for pagination. (default is 1) <br/>  limit: the maximum number of records to return (Max is 50 default to 50) </remarks>
        /// <param name="getListsDTO">query paramter DTO</param>
        /// <param name="searchString">pass an optional search string for looking up a list</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">List not found</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ToDoDTO>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get([FromQuery] GetListsDTO getListsDTO, [FromQuery]string searchString)
        {
            try
            {
                if (!ModelState.IsValid || getListsDTO.Id.Equals(0))
                    return BadRequest();

                var list = _cache.GetCachedObject<IEnumerable<ToDoDTO>>($"user-{getListsDTO.Id}");

                if (list == null)
                    list = await _service.GetToDoByPaging(getListsDTO.Id, getListsDTO.Skip, getListsDTO.Limit, searchString);

                if (!list.Any())
                    return NotFound();

                var rowVersion = list.FirstOrDefault().RowVersion;
                var isChanged = _cache.SetCachedObject($"user-{getListsDTO.Id}", list, new byte[] {0,4 }, 3);
                if (isChanged)
                {
                    return Ok(list);
                }
                else
                {                    
                    return StatusCode(StatusCodes.Status304NotModified);
                }
             
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ToDo list");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// return the specified todo list
        /// </summary>
        /// <remarks>Parameters</remarks>   
        /// <param name="id">The unique identifier of the list</param>
        /// <response code="200">successful operation</response>
        /// <response code="400">Invalid id supplied</response>
        /// <response code="404">List not found</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [ProducesResponseType(200, Type = typeof(ToDoDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var toDo = await _service.GetToDoById(id);

                if (toDo == null)
                    return NotFound();

                return Ok(toDo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo by id");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// creates a new list
        /// </summary>
        /// <remarks>Adds a list to the system</remarks>
        /// <param name="toDo">ToDo list to add</param>
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
        public async Task<IActionResult> Create([FromBody] CreateDTO toDo)
        {
            try
            {
                if (toDo == null || !ModelState.IsValid)
                    return BadRequest();

                var Id = await _service.CreateToDo(toDo);

                if (Id.Equals(0))
                    return StatusCode(StatusCodes.Status409Conflict);

                return CreatedAtRoute("Get", new { id = Id }, toDo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating ToDo");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update to do Name and Description
        /// </summary>
        /// <remarks>update to do</remarks>
        /// <param name="toDo">to do to update</param>
        /// <response code="201">to do updated</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>       
        [HttpPut]
        [ProducesResponseType(201, Type = typeof(ToDoDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Put([FromBody]ToDoDTO toDo)
        {
            try
            {
                if (!ModelState.IsValid || toDo == null)
                    return BadRequest();

                var update = await _service.UpdateToDo(toDo);

                if (!update)
                    return NotFound();

                return CreatedAtRoute("Get", new { id = toDo.Id }, toDo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating ToDo");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Update to do Name
        /// </summary>
        /// <remarks>update to do name</remarks>
        /// <param name="id">To do id</param>
        /// <param name="name">To do name</param>
        /// <response code="201">to do updated</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>no content</returns>
        [HttpPut("{id}/update/name")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutUpdateName(int id, [FromBody]string name)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(name))
                    return BadRequest();

                var update = await _service.UpdateToDoName(id, name);

                return CreatedAtRoute("Get", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update ToDo Name");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update to do description
        /// </summary>
        /// <remarks>update to do description</remarks>
        /// <param name="id">To do id</param>
        /// <param name="description">To do name</param>
        /// <response code="201">to do updated</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>no content</returns>
        [HttpPut("{id}/update/description")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutUpdateDescription(int id, [FromBody]string description)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(description))
                    return BadRequest();

                var update = await _service.UpdateToDoDescription(id, description);

                return CreatedAtRoute("Get", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update ToDo Name");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}
