using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class PutTest
    {
        IToDoService _moqService;
        ILogger<ToDoController> _mockLogger;
        IETagCache _mockCache;
        IToDoUpdateDTO _mockToDoUpdateDTO;
        IUpdateDescriptionDTO _mockUpdateDescriptionDTO;
        IUpdateNameDTO _mockUpdateNameDTO;
        IModelValidation _mockModelValidation;
        ToDoController _toDoController;
        readonly int _toDoId = 42;

        [OneTimeSetUp]
        public void OneSetUp()
        {
            _moqService = new Mock<IToDoService>().Object;
            _mockLogger = new Mock<ILogger<ToDoController>>().Object;
            _mockCache = new Mock<IETagCache>().Object;
            _mockModelValidation = new Mock<ModelValidation>().Object;
        }

        [SetUp]
        public void SetUp()
        {
            _toDoController = new ToDoController(_moqService, _mockLogger, _mockCache);
            _mockToDoUpdateDTO = new Mock<ToDoUpdateDTO>().Object;         
            _mockToDoUpdateDTO.UserId = 14321;
            _mockToDoUpdateDTO.Name = "For the horde";
            _mockToDoUpdateDTO.Description = "battle for azeroth";

            _mockUpdateDescriptionDTO = new Mock<UpdateDescriptionDTO>().Object;
            _mockUpdateDescriptionDTO.UserId = 14321;
            _mockUpdateDescriptionDTO.Description = "Protect the dragon qween";

            _mockUpdateNameDTO = new Mock<UpdateNameDTO>().Object;
            _mockUpdateNameDTO.UserId = 14321;
            _mockUpdateNameDTO.Name = "Emilia Clarke";
        }
       
        /// <summary>
        /// If id is negative number return 400 error
        /// </summary>
        [Test]
        public async Task PutFourHundredResponseWhenIdIsNegative()
        {
            //Arange        
            var toDoId = -1;
            //Act
            var actionResult = await _toDoController.Put(toDoId, It.IsAny<ToDoUpdateDTO>());
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        /// <summary>
        /// test 404 response
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutFourHundredFourResponse()
        {
            //Arrange
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.UpdateToDo(It.IsAny<int>(), It.IsAny<IToDoUpdateDTO>())
                ).ReturnsAsync(false);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Put(_toDoId, _mockToDoUpdateDTO as ToDoUpdateDTO);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// test  no content response update to do
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutNoContentResponse()
        {
            //Arrange
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.UpdateToDo(It.IsAny<int>(), It.IsAny<IToDoUpdateDTO>())
                ).ReturnsAsync(true);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.Put(_toDoId, _mockToDoUpdateDTO as ToDoUpdateDTO);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);       
        }

        /// <summary>
        /// If id is negative number return 400 error
        /// </summary>
        [Test]
        public async Task PutNameFourHundredResponseWhenIdIsNegative()
        {
            //Arange  
            var toDoId = -1;     
            //Act
            var actionResult = await _toDoController.PutUpdateName(toDoId, It.IsAny<UpdateNameDTO>());
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }        

        /// <summary>
        /// test 404 response update name
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutNameFourHundredFourResponse()
        {
            //Arrange        
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.UpdateToDoName(It.IsAny<int>(), It.IsAny<UpdateNameDTO>())
                ).ReturnsAsync(false);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.PutUpdateName(_toDoId, It.IsAny<UpdateNameDTO>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// test  no content response update to do name
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutNameNoContentResponse()
        {
            //Arrange    
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.UpdateToDoName(It.IsAny<int>(), It.IsAny<UpdateNameDTO>())
                ).ReturnsAsync(true);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.PutUpdateName(_toDoId, It.IsAny<UpdateNameDTO>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        /// <summary>
        /// If id is negative number return 400 error
        /// </summary>
        [Test]
        public async Task PutDescriptionFourHundredResponseWhenIdIsNegative()
        {
            //Arange  
            var toDoId = -1;   
            //Act
            var actionResult = await _toDoController.PutUpdateDescription(toDoId, It.IsAny<UpdateDescriptionDTO>());
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }        

        /// <summary>
        /// test 404 response update description
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutDescriptionFourHundredFourResponse()
        {
            //Arrange       
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.UpdateToDoDescription(It.IsAny<int>(), It.IsAny<UpdateDescriptionDTO>())
                ).ReturnsAsync(false);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.PutUpdateDescription(_toDoId, It.IsAny<UpdateDescriptionDTO>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        /// <summary>
        /// test description  no content response update to do
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task PutDescriptionNoContentResponse()
        {
            //Arrange           
            var mockService = new Mock<IToDoService>();
            mockService.Setup(
                service => service.UpdateToDoDescription(It.IsAny<int>(), It.IsAny<UpdateDescriptionDTO>())
                ).ReturnsAsync(true);
            var controller = new ToDoController(mockService.Object, _mockLogger, _mockCache);
            //Act
            var result = await controller.PutUpdateDescription(_toDoId, It.IsAny<UpdateDescriptionDTO>());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        #region Model Validation

        /// <summary>
        /// Update DTO UserId less then 1
        /// </summary>
        [Test]
        public void PutToDoFourHundredWhenUserIdIsZero()
        {
            //Arrange
            _mockToDoUpdateDTO.UserId = 0;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockToDoUpdateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field UserId must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }        

        /// <summary>
        /// update DTO Name is null
        /// </summary>
        [Test]
        public void PutToDoFourHundredWhenNameIsNull()
        {
            //Arrange
            _mockToDoUpdateDTO.Name = null;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockToDoUpdateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The Name field is required."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Create DTO Name can't exceed 255 in length
        /// </summary>
        [Test]
        public void PutToDoFourHundredWhenNameExceedMax()
        {
            //Arrange
            _mockToDoUpdateDTO.Name = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockToDoUpdateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Name must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Create DTO Description can't exceed 255 in length
        /// </summary>
        [Test]
        public void PutToDoFourHundredWhenDescriptionExceedMax()
        {
            //Arrange
            _mockToDoUpdateDTO.Description = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockToDoUpdateDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Description must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Update Name DTO UserId less then 1
        /// </summary>
        [Test]
        public void PutNameFourHundredWhenUserIdIsZero()
        {
            //Arrange
            _mockUpdateNameDTO.UserId = -1532;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockUpdateNameDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field UserId must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Update Name DTO name can't exceed 255 in length
        /// </summary>
        [Test]
        public void PutNameFourHundredWhenNameExceedMax()
        {
            //Arrange
            _mockUpdateNameDTO.Name = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockUpdateNameDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Name must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Update Description DTO UserId less then 1
        /// </summary>
        [Test]
        public void PutDescriptionFourHundredWhenUserIdIsZero()
        {
            //Arrange
            _mockUpdateDescriptionDTO.UserId = -1532;
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockUpdateDescriptionDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field UserId must be between 1 and 2147483647."));
            //Assert
            Assert.IsTrue(actual);
        }

        /// <summary>
        /// Update Name DTO name can't exceed 255 in length
        /// </summary>
        [Test]
        public void PutDescriptionFourHundredWhenDescriptionExceedMax()
        {
            //Arrange
            _mockUpdateDescriptionDTO.Description = "In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act        
            var errors = _mockModelValidation.ValidateModels(_mockUpdateDescriptionDTO);
            var actual = errors.Any(e => e.ErrorMessage.Contains("The field Description must be a string with a maximum length of 255."));
            //Assert
            Assert.IsTrue(actual);
        }
        #endregion
    }
}
