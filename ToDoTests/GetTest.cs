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
    public class GetTest
    {
        Mock<IToDoService> _mockService;
        ILogger<ToDoController> _mockLogger;
        IETagCache _mockCache;
        ToDoController _toDoController;
        GetListsDTO getListDTO;
        IModelValidation _mockModelValidation;
        ToDoDTO _toDoDTO;
        string searchString = string.Empty;
        int userId = 123;
        int skip = 1;
        int limit = 25;
        int toDoId = 132213;

        [OneTimeSetUp]
        public void OneSetUp()
        {            
            _mockLogger = new Mock<ILogger<ToDoController>>().Object;
            _mockCache = new Mock<IETagCache>().Object;
            _toDoController = new ToDoController(new Mock<IToDoService>().Object, _mockLogger, _mockCache);
            _mockModelValidation = new Mock<ModelValidation>().Object;
        }

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IToDoService>();
            _toDoDTO = new ToDoDTO
            {
                Id = toDoId,
                Name = "Protect Qween Of The South",
                Description = "Guard"
            };
            getListDTO = new GetListsDTO
            {
                Id = userId,
                Skip = skip,
                Limit = limit
            };
        }

        /// <summary>
        /// Get User lists return 200
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserListsTwoHundredResponse()
        {
            //Arrange                 
            var list = new List<ToDoDTO>();
            list.Add(new ToDoDTO
            {
                Description = "Protect",
                Name = "Protect Queen of the south",
                Id = 12154                
            });            

            _mockService.Setup(
                service => service.GetToDoByPaging(userId, skip, limit, searchString)
                ).ReturnsAsync(list);
            //Act
            var controller = new ToDoController(_mockService.Object, _mockLogger, _mockCache);
            var result = await controller.Get(getListDTO, searchString);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        /// <summary>
        /// Get list return null unable to find to do with that user id
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserListsNotFound()
        {
            //Arrange  
            _mockService.Setup(
                service => service.GetToDoByPaging(userId, skip, limit, searchString)
                ).ReturnsAsync((IEnumerable<ToDoDTO>)(null));
            var controller = new ToDoController(_mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Get(getListDTO, searchString);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// Get To Do by ID return ok response
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserToDoByIdReturnTwoHundred()
        {
            //Arrange  
            _mockService.Setup(
                service => service.GetToDoById(toDoId)
                ).ReturnsAsync(_toDoDTO);
            var controller = new ToDoController(_mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Get(toDoId);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        /// <summary>
        /// Get To Do by ID return not found
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task GetUserToDoByIdReturnNotFound()
        {
            //Arrange  
            _mockService.Setup(
                service => service.GetToDoById(toDoId)
                ).ReturnsAsync((IToDoDTO)null);
            var controller = new ToDoController(_mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Get(toDoId);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        #region Model Test     
        [Test]
        public void GetListsFourHundredWhenIdIsNegativeNumber()
        {
            //Arrange
            getListDTO.Id = -1;
            //Act        
            var errors = _mockModelValidation.ValidateModels(getListDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Id must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void GetListsFourHundredWhenSkipIsNegative()
        {
            //Arrange
            getListDTO.Skip = -1;
            //Act        
            var errors = _mockModelValidation.ValidateModels(getListDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Skip must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void GetListsFourHundredWhenLimitOutOfRange()
        {
            //Arrange
            getListDTO.Limit = 51;          
            //Act        
            var errors = _mockModelValidation.ValidateModels(getListDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Limit must be between 1 and 50."));
            //Assert
            Assert.IsTrue(actual);
        }        
        #endregion
    }
}
