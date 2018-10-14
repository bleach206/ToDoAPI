using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using Common.Interface;
using Model;
using Model.Interface;
using Service.Interface;
using ToDoAPI.Controllers;
using ToDoTests.Interface;

namespace ToDoTests
{
    [TestFixture]
    public class PostTest
    {
        IToDoService _moqService;
        ILogger<ToDoController> _mockLogger;
        IETagCache _mockCache;        
        ICreateDTO _mockCreateDTO;
        List<CreateDTO> _mockCreateList;
        IModelValidation _mockModelValidation;
        ToDoController _toDoController;       

        int userId = 123;

        [OneTimeSetUp]
        public void OneSetUp()
        {
            _moqService = new Mock<IToDoService>().Object;
            _mockLogger = new Mock<ILogger<ToDoController>>().Object;
            _mockCache = new Mock<IETagCache>().Object;
            _mockModelValidation = new Mock<ModelValidation>().Object;
            _mockCreateList = new List<CreateDTO>();
        }

        [SetUp]
        public void SetUp()
        {
            _toDoController = new ToDoController(_moqService, _mockLogger, _mockCache);
            var task = new Mock<CreateTaskDTO>().Object;
            task.Name = "Guard and kill everyone else";           

            _mockCreateDTO = new Mock<CreateDTO>().Object;

            _mockCreateDTO.Name = "Protect Daenerys Stormborn";
            _mockCreateDTO.Description = "Protecter of Dragons";
        }

        #region Model Validation
        /// <summary>
        /// Create DTO Name is null
        /// </summary>
        [Test]
        public void CreateToDoFourHundredWhenNameIsNull()
        {
            //Arrange
            _mockCreateDTO.Name = null;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockCreateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// To Do name can't be empty
        /// </summary>
        [Test]
        public void CreateFourHundredWhenToDoListNameIsEmpty()
        {
            //Arrange          
            _mockCreateDTO.Name = string.Empty;
            var errors = _mockModelValidation.ValidateModels(_mockCreateDTO);
            //Act
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// To Do name can't exceed 255 in length
        /// </summary>
        [Test]
        public void CreateFourHundredWhenToDoListNameExceedMax()
        {
            //Arrange           
            _mockCreateDTO.Name = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act
            var errors = _mockModelValidation.ValidateModels(_mockCreateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Name must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// To Do  description can't exceed 255 in length
        /// </summary>
        [Test]
        public void CreateFourHundredWhenToDoListDescriptionExceedMax()
        {
            //Arrange               
            _mockCreateDTO.Description = @"In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act
            var errors = _mockModelValidation.ValidateModels(_mockCreateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Description must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }
        #endregion      

        /// <summary>
        /// If Dto is null return 400 error
        /// </summary>
        [Test]
        public async Task CreateListFourHundredResponseWhenDtoIsNull()
        {
            //Arange  
            //Act
            var actionResult = await _toDoController.Create(userId, null);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }          

        /// <summary>
        /// create to do with created route
        /// </summary>
        [Test]
        public async Task CreateToDoReturnCreatedRoute()
        {
            //Arrange  
            var toDoId = 41574;
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.CreateToDo(It.IsAny<int>(), It.IsAny<CreateDTO>())
                ).ReturnsAsync(toDoId);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Create(userId, _mockCreateDTO as CreateDTO);
            var resultResponse = result as CreatedAtRouteResult;
            //Assert
            Assert.IsInstanceOf<CreatedAtRouteResult>(resultResponse);
            Assert.AreEqual("Get", resultResponse.RouteName);
        }

        /// <summary>
        /// create to do with return 409 response conflict
        /// </summary>
        [Test]
        public async Task CreateToDoFourHundredNineResponse()
        {
            //Arrange  
            var expected = StatusCodes.Status409Conflict;
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.CreateToDo(It.IsAny<int>(), _mockCreateDTO)
                ).ReturnsAsync(0);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Create(userId, _mockCreateDTO as CreateDTO) as StatusCodeResult;
            //Assert
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(expected, result.StatusCode);
        }

        /// <summary>
        /// create to do lists with created route
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CreateToDoListsReturnCreatedRoute()
        {
            //Arrange  
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.CreateToDoList(It.IsAny<int>(), It.IsAny<IEnumerable<CreateDTO>>())
                ).ReturnsAsync(userId);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);          
            //Act
            var result = await controller.CreateList(userId, _mockCreateList);
            var resultResponse = result as CreatedAtRouteResult;
            //Assert
            Assert.IsInstanceOf<CreatedAtRouteResult>(resultResponse);
            Assert.AreEqual("Get", resultResponse.RouteName);
        }

        /// <summary>
        /// create to do lists bad request response
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CreateToDoListsBadRequestResponse()
        {
            //Arrange  
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.CreateToDoList(It.IsAny<int>(), It.IsAny<IEnumerable<CreateDTO>>())
                ).ReturnsAsync(userId);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.CreateList(userId, It.IsAny<IEnumerable<CreateDTO>>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        /// <summary>
        /// create to do list with return 409 response conflict
        /// </summary>
        [Test]
        public async Task CreateToDoListFourHundredNineResponse()
        {
            //Arrange  
            var expected = StatusCodes.Status409Conflict;
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.CreateToDoList(It.IsAny<int>(), It.IsAny<IEnumerable<CreateDTO>>())
                ).ReturnsAsync(0);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.CreateList(userId, _mockCreateList) as StatusCodeResult;
            //Assert
            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(expected, result.StatusCode);
        }
    }
}
