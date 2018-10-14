using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using Common.Interface;
using Service.Interface;
using ToDoAPI.Controllers;

namespace ToDoTests
{
    [TestFixture]
    public class DeleteTest
    {
        IToDoService _moqService;
        ILogger<ToDoController> _mockLogger;
        IETagCache _mockCache;
        ToDoController _toDoController;
        readonly int _toDoId = 124321;
        readonly int _userId = 12423;

        [OneTimeSetUp]
        public void OneSetUp()
        {
            _moqService = new Mock<IToDoService>().Object;
            _mockLogger = new Mock<ILogger<ToDoController>>().Object;
            _mockCache = new Mock<IETagCache>().Object;
        }

        [SetUp]
        public void SetUp()
        {
            _toDoController = new ToDoController(_moqService, _mockLogger, _mockCache);
        }
 
        /// <summary>
        /// If id is negative number return 400 error
        /// </summary>
        [Test]
        public async Task DeleteFourHundredResponseWhenIdIsNegative()
        {
            //Arange        
            var toDoId = -1;
            var userId = 1242;
            //Act
            var actionResult = await _toDoController.Delete(toDoId, userId);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        /// <summary>
        /// test 404 response
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeleteFourHundredFourResponse()
        {
            //Arrange          
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.Delete(It.IsAny<int>(), It.IsAny<int>())
                ).ReturnsAsync(false);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Delete(_toDoId, _userId);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// test  no content response delete to do
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeleteNoContentResponse()
        {
            //Arrange           
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.Delete(It.IsAny<int>(), It.IsAny<int>())
                ).ReturnsAsync(true);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Delete(_toDoId, _userId);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}
