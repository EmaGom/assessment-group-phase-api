using Assessment.Group.Phase.Controllers;
using Assessment.Group.Phase.Managers;
using Assessment.Group.Phase.Models;
using Assessment.Group.Phase.Models.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Assassment.Group.Phase.Test.ControllersTests
{
    [TestFixture]
    public class GroupSimulatorControllerTest
    {
        private Mock<ILogger<GroupSimulatorController>> _logger;
        private Mock<IGroupManager> _groupManager;
        private Mock<ITeamManager> _teamManager;
        private GroupSimulatorController _sut;

        [SetUp]
        public void init()
        {
            _logger = new Mock<ILogger<GroupSimulatorController>>();
            _groupManager = new Mock<IGroupManager>();
            _teamManager = new Mock<ITeamManager> ();

            _sut = new GroupSimulatorController(_logger.Object, _groupManager.Object, _teamManager.Object);
        }

        [TearDown]
        public void destroy()
        {
            _logger = null;
            _groupManager = null;
            _teamManager = null;
            _sut = null;
        }

        #region GetGroup
        [Test]
        [Category("GetGroup")]
        public void Get_Success() => this.GenericGet(false, StatusCodes.Status200OK);

        [Test]
        [Category("GetGroup")]
        public void Get_BadRequest() => this.GenericGet(false, StatusCodes.Status400BadRequest);

        [Test]
        [Category("GetGroup")]
        public void Get_InternalServerError() => this.GenericGet(true, StatusCodes.Status500InternalServerError);
        private void GenericGet(bool exceptionThrown, int expectedStatusCode)
        {
            // Arrange
            var response = new GroupEntity()
            {
                Id = 1,
                Matches = new List<Assessment.Group.Phase.Models.Match>(),
                Scorers = new List<Player>(),
                Assistants = new List<Player>(),
                TeamStats = new List<TeamStats>()
            };

            var mockSetTeams = _teamManager.Setup(x => x.SetTeams());
            var mockSaveGroupGroup = _groupManager.Setup(x => x.CreateGroup());
            var mockSimulateMatchDay = _groupManager.Setup(x => x.SimulateMatchDay(It.IsAny<int>()));
            var mockGetGroup = _groupManager.Setup(x => x.GetGroup());

            if (exceptionThrown)
                mockSetTeams.Throws(new Exception());
            else if (expectedStatusCode == StatusCodes.Status400BadRequest)
                mockSetTeams.Throws(new BadRequestException());
            else
                mockGetGroup.Returns(response);


            // Act
            IActionResult result = this._sut.GetGroup();

            // Assert
            this.Generic_Asserts(result, expectedStatusCode);
        }
        #endregion

        #region SaveGroup
        [Test]
        [Category("SaveGroup")]
        public void SaveGroup_Success() => this.GenericSaveGroup(new GroupEntity() { Id = 0 }, false, StatusCodes.Status201Created);

        [Test]
        [Category("SaveGroup")]
        public void SaveGroup_BadRequest() => this.GenericSaveGroup(null, false, StatusCodes.Status400BadRequest);

        [Test]
        [Category("SaveGroup")]
        public void SaveGroup_InternalServerError() => this.GenericSaveGroup(new GroupEntity() { Id = 0 }, true, StatusCodes.Status500InternalServerError);

        private void GenericSaveGroup(GroupEntity groupEntity, bool exceptionThrown, int expectedStatusCode)
        {
            // Arrange
            var response = new GroupEntity()
            {
                Id = 1
            };

            var mockSaveGroup = _groupManager.Setup(x => x.SaveGroup(It.IsAny<GroupEntity>()));

            if (exceptionThrown)
                mockSaveGroup.Throws(new Exception());
            else if (expectedStatusCode == StatusCodes.Status400BadRequest)
                mockSaveGroup.Throws(new BadRequestException());
            else
                mockSaveGroup.Returns(response);

            // Act
            IActionResult result = _sut.SaveGroup(groupEntity);

            // Assert
            this.Generic_Asserts(result, expectedStatusCode);
        }
        #endregion

        #region GetSimulationHistory
        [Test]
        [Category("GetGroupsHistory")]
        public void GetGroupsHistory_Success() => this.GenericGetGroupsHistory(false, StatusCodes.Status200OK);

        [Test]
        [Category("GetGroupsHistory")]
        public void GetGroupsHistory_InternalServerError() => this.GenericGetGroupsHistory(true, StatusCodes.Status500InternalServerError);
        private void GenericGetGroupsHistory(bool exceptionThrown, int expectedStatusCode)
        {
            // Arrange
            var response = new List<GroupEntity>()
            {
                new GroupEntity()
                {
                    Id = 1,
                    Matches = new List<Assessment.Group.Phase.Models.Match>(),
                    Scorers = new List<Player>(),
                    Assistants = new List<Player>(),
                    TeamStats = new List<TeamStats>()
                }
            };


            var mockGetGroupHistory = _groupManager.Setup(x => x.GetGroupsHistory(10));

            if (exceptionThrown)
                mockGetGroupHistory.Throws(new Exception());
            else
                mockGetGroupHistory.Returns(response);


            // Act
            IActionResult result = this._sut.GetGroupsHistory();

            // Assert
            this.Generic_Asserts(result, expectedStatusCode);
        }
        #endregion

        protected void Generic_Asserts(IActionResult result, int expectedStatusCode)
        {
            // Assert
            switch (expectedStatusCode)
            {
                case 200:
                    result.Should().BeOfType<OkObjectResult>();
                    result.As<OkObjectResult>().StatusCode.Should().Be(expectedStatusCode);
                    break;
                case 201:
                    result.Should().BeOfType<ObjectResult>();
                    result.As<ObjectResult>().StatusCode.Should().Be(expectedStatusCode);
                    break;
                case 400:
                    result.Should().BeOfType<BadRequestObjectResult>();
                    result.As<BadRequestObjectResult>().StatusCode.Should().Be(expectedStatusCode);
                    break;
                case 404:
                    result.Should().BeOfType<NotFoundResult>();
                    result.As<NotFoundResult>().StatusCode.Should().Be(expectedStatusCode);
                    break;
                case 500:
                    result.As<ObjectResult>().StatusCode.Should().Be(expectedStatusCode);
                    break;
                default:
                    Assert.Fail($"Expected {expectedStatusCode}");
                    break;
            }
        }

    }
}
