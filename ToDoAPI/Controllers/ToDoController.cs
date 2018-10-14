using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Common.Interface;
using Model;
using Service.Interface;

namespace ToDoAPI.Controllers
{
    /// <summary>
    /// Doing the things that need to be done
    /// </summary>
    [Route("api/v1/todo")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        #region Fields

        private readonly IToDoService _service;
        private readonly ILogger _logger;
        private readonly IETagCache _cache;
        #endregion

        private const int _cacheTimeMinutes = 3;

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
        /// <param name="id">user Id</param>
        /// <param name="getListsDTO">query paramter DTO</param>       
        /// <response code="200">successful operation</response>
        /// <response code="304">Not Modified</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">List not found</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpGet("user/{id}/lists")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ToDoDTO>))]
        [ProducesResponseType(304)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Get(int id, [FromQuery] GetListsDTO getListsDTO)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0)
                    return BadRequest();

                var list = _cache.GetCachedObject<IEnumerable<ToDoDTO>>($"user-{id}");

                if (list == null)
                    list = await _service.GetToDoByPaging(id, getListsDTO.Skip, getListsDTO.Limit, getListsDTO.SearchString);

                if (!list.Any())
                    return NotFound();
       
                var rowVersion = list.FirstOrDefault().RowVersion;
                var isChanged = _cache.SetCachedObject($"user-{id}", list, rowVersion, _cacheTimeMinutes);
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
        /// return the specified todo
        /// </summary>
        /// <remarks>Parameters</remarks>   
        /// <param name="id">User Id</param>
        /// <param name="toDoId">The unique identifier of to do</param>
        /// <response code="200">successful operation</response>
        /// <response code="304">Not Modified</response>
        /// <response code="400">Invalid id supplied</response>
        /// <response code="404">List not found</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [ProducesResponseType(200, Type = typeof(ToDoDTO))]
        [ProducesResponseType(304)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet("user/{id}/list/{toDoId}", Name = "Get")]
        public async Task<IActionResult> Get(int id, int toDoId)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0 || toDoId <= 0)
                    return BadRequest();

                var toDo = _cache.GetCachedObject<ToDoDTO>($"todo-{toDoId}");

                if (toDo == null)
                    toDo = await _service.GetToDoById(id, toDoId);

                if (toDo == null)
                    return NotFound();

                var isChanged = _cache.SetCachedObject($"todo-{toDoId}", toDo, toDo.RowVersion, _cacheTimeMinutes);
                if (isChanged)
                {
                    return Ok(toDo);
                }
                else
                {
                    return StatusCode(StatusCodes.Status304NotModified);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo by id");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// creates a to do
        /// </summary>
        /// <remarks>Adds a to do into the system</remarks>
        /// <param name="id">User Id</param>
        /// <param name="toDo">ToDo list to add</param>
        /// <response code="201">item created</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="409">an existing item already exists</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpPost("user/{id}")]
        [ProducesResponseType(201, Type = typeof(CreateDTO))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create(int id, [FromBody] CreateDTO toDo)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0 || toDo == null)
                    return BadRequest();

                var Id = await _service.CreateToDo(id, toDo);

                if (Id.Equals(0))
                    return StatusCode(StatusCodes.Status409Conflict);

                return CreatedAtRoute("Get", new { id, toDoId = Id }, toDo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating ToDo");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// creates a to do list
        /// </summary>
        /// <remarks>Adds a to do list into the system</remarks>
        /// <param name="id">User Id</param>
        /// <param name="list">To do lists</param>
        /// <response code="201">item created</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="409">an existing item already exists</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>
        [HttpPost("user/{id}/lists")]
        [ProducesResponseType(201, Type = typeof(IEnumerable<CreateDTO>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateList(int id, [FromBody] IEnumerable<CreateDTO> list)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0 || list == null)
                    return BadRequest();

                var Id = await _service.CreateToDoList(id, list);

                if (Id.Equals(0))
                    return StatusCode(StatusCodes.Status409Conflict);

                return CreatedAtRoute("Get", id, list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating to do lists");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update to do Name and Description
        /// </summary>
        /// <remarks>update to do</remarks>
        /// <param name="id">todo id</param>
        /// <param name="toDo">to do to update</param>
        /// <response code="204">to do updated</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>Response code and dto object</returns>       
        [HttpPut("update/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Put(int id, [FromBody]ToDoUpdateDTO toDo)
        {
            try
            {
                if (!ModelState.IsValid || toDo == null || id <= 0)
                    return BadRequest();

                var update = await _service.UpdateToDo(id, toDo);

                if (!update)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error put ToDo");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Update to do Name
        /// </summary>
        /// <remarks>update to do name</remarks>
        /// <param name="id">To do id</param>
        /// <param name="updateNameDTO"> dto holding name and user id</param>        
        /// <response code="204">to do updated</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>no content</returns>
        [HttpPut("update/name/{id}/")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutUpdateName(int id, [FromBody]UpdateNameDTO updateNameDTO)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0 )
                    return BadRequest();

                var update = await _service.UpdateToDoName(id, updateNameDTO);

                if (!update)
                    return NotFound();

                return NoContent();
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
        /// <param name="updateDescriptionDTO">dto for user id and description</param>   
        /// <response code="204">to do updated</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>no content</returns>
        [HttpPut("update/description/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutUpdateDescription(int id, [FromBody]UpdateDescriptionDTO updateDescriptionDTO)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0)
                    return BadRequest();

                var update = await _service.UpdateToDoDescription(id, updateDescriptionDTO);

                if (!update)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update ToDo description");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// delete to do
        /// </summary>
        /// <remarks>delete to do</remarks>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <response code="204">to do deleted</response>
        /// <response code="400">invalid input, object invalid</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">server error</response>
        /// <returns>no content</returns>
        [HttpDelete("{id}/user/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int id, int userId)
        {
            try
            {
                if (!ModelState.IsValid || id <= 0)
                    return BadRequest();

                var deleted = await _service.Delete(id, userId);

                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete to do");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
