using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

using Moq;
using NUnit.Framework;

using Common.Interface;
using Model;
using Model.Interface;
using Service.Interface;
using ToDoAPI.Controllers;

namespace ToDoTests
{
    [TestFixture]
    public class PostTest
    {
        IToDoService _moqService;
        ILogger<ToDoController> _mockLogger;
        IETagCache _mockCache;
        ToDoController _toDoController;
        ICreateDTO _mockCreateDTO;


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
            var task = new Mock<CreateTaskDTO>().Object;
            task.Name = "Guard and kill everyone else";
            var list = new List<CreateTaskDTO>();
            list.Add(task);

            _mockCreateDTO = new Mock<CreateDTO>().Object;

            _mockCreateDTO.Name = "Protect Daenerys Stormborn";
            _mockCreateDTO.Description = "Protecter of Dragons";

            _mockCreateDTO.Tasks = list;
        }

        #region CreateDTO Post Test

        /// <summary>
        /// If Dto is null return 400 error
        /// </summary>
        [Test]
        public void CreateListFourHundredResponseWhenDtoIsNull()
        {
            //Arange  
            //Act
            var actionResult = _toDoController.Create(null);

            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult.Result);
        }

        /// <summary>
        /// To Do list name can't be null
        /// </summary>
        [Test]
        public void CreateListFourHundredWhenToDoListNameIsNull()
        {
            //Arrange        
            _mockCreateDTO.Name = null;
            _toDoController.ModelState.AddModelError("Name", "ConvertEmptyStringToNull");
            //Act
            var actualResult = _toDoController.Create(_mockCreateDTO as CreateDTO);
            //Assert          
            Assert.IsInstanceOf<BadRequestResult>(actualResult.Result);
        }

        /// <summary>
        /// To Do list name can't be empty
        /// </summary>
        [Test]
        public void CreateListFourHundredWhenToDoListNameIsEmpty()
        {
            //Arrange          
            _mockCreateDTO.Name = string.Empty;
            _toDoController.ModelState.AddModelError("Name", "AllowEmptyStrings");
            //Act
            var actualResult = _toDoController.Create(_mockCreateDTO as CreateDTO);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actualResult.Result);
        }

        /// <summary>
        /// To Do list name can't exceed 255 in length
        /// </summary>
        [Test]
        public void CreateListFourHundredWhenToDoListNameExceedMax()
        {
            //Arrange           
            _toDoController.ModelState.AddModelError("Name", "StringLength");
            _mockCreateDTO.Name = @"In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act
            var actualResult = _toDoController.Create(_mockCreateDTO as CreateDTO);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actualResult.Result);

        }

        /// <summary>
        /// To Do list description can't exceed 255 in length
        /// </summary>
        [Test]
        public void CreateListFourHundredWhenToDoListDescriptionExceedMax()
        {
            //Arrange           
            _toDoController.ModelState.AddModelError("Description", "StringLength");
            _mockCreateDTO.Description = @"In the story, Daenerys is a young woman in her early teens living in Essos across the Narrow Sea. Knowing no other life than one of exile, she remains dependent on her abusive older brother, Viserys. The timid and meek girl finds herself married to Dothraki horselord Khal Drogo, in exchange for an army for Viserys which is to return to Westeros and recapture the Iron Throne. Despite this, her brother loses the ability to control her as Daenerys finds herself adapting to life with the khalasar and emerges as a strong, confident and courageous woman. She becomes the heir of the Targaryen dynasty after her brother's death and plans to reclaim the Iron Throne herself, seeing it as her birthright. A pregnant Daenerys loses her husband and child, but soon helps hatch three dragons from their eggs, which regard her as their mother, providing her with a tactical advantage and prestige";
            //Act
            var actualResult = _toDoController.Create(_mockCreateDTO as CreateDTO);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actualResult.Result);

        }

        /// <summary>
        /// To Do list Task can't be null
        /// </summary>
        [Test]
        public void CreateListFourHundredWhenToDoListTaskCanNotBeNull()
        {
            //Arrange         
            _mockCreateDTO.Tasks = null;
            _toDoController.ModelState.AddModelError("Tasks", "ConvertEmptyStringToNull");
            //Act
            var actualResult = _toDoController.Create(_mockCreateDTO as CreateDTO);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actualResult.Result);
        }
        #endregion

        #region CreateTaskDTO Post Test

        /// <summary>
        /// To Do task name can't be null
        /// </summary>
        [Test]
        public void CreateListFourHundredWhenToDoTaskNameIsNull()
        {
            //Arrange          
            _mockCreateDTO.Tasks.FirstOrDefault().Name = null;
            _toDoController.ModelState.AddModelError("Name", "ConvertEmptyStringToNull");
            //Act
            var actualResult = _toDoController.Create(_mockCreateDTO as CreateDTO);
            //Assert
            Assert.IsInstanceOf<BadRequestResult>(actualResult.Result);
        }
        #endregion

        #region Post

        [Test]
        public void CreateToDoReturnCreatedRoute()
        {
            //Arrange   
            //Act
            var result = _toDoController.Create(_mockCreateDTO as CreateDTO);
            var resultResponse = result.Result as CreatedAtRouteResult;
            //Assert
            Assert.IsInstanceOf<CreatedAtRouteResult>(resultResponse);
            Assert.AreEqual("Get", resultResponse.RouteName);
        }
        #endregion
    }
}
