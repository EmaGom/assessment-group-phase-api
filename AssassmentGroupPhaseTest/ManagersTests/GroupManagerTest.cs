using Assessment.Group.Phase.Enums;
using Assessment.Group.Phase.Helpers;
using Assessment.Group.Phase.Managers;
using Assessment.Group.Phase.Models;
using Assessment.Group.Phase.Models.Exceptions;
using Assessment.Group.Phase.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assassment.Group.Phase.Test.ManagersTests
{
    public class GroupManagerTest
    {
        private Mock<ILogger<GroupManager>> _logger;
        private Mock<ISimulationService> _simulationService;
        private Mock<ITeamManager> _teamManager;
        private Mock<ICacheService> _cacheService;
        private Mock<IRepository> _repository;
        private Mock<IGroupRepository> _groupRepository;
        private GroupManager _sut;

        [SetUp]
        public void Init()
        {
            _logger = new Mock<ILogger<GroupManager>>();
            _cacheService = new Mock<ICacheService>();
            _teamManager = new Mock<ITeamManager>();
            _simulationService = new Mock<ISimulationService>();
            _repository = new Mock<IRepository>();
            _groupRepository = new Mock<IGroupRepository>();

            _sut = new GroupManager(_logger.Object, _cacheService.Object, _teamManager.Object, _simulationService.Object, _repository.Object, _groupRepository.Object);
        }

        [TearDown]
        public void destroy()
        {
            this._teamManager = null;
            this._cacheService = null;
            this._simulationService = null;
            this._groupRepository = null;
            this._sut = null;
        }

        #region CreateGroup
        [Test]
        [Category("CreateGroup")]
        public void CreateGroup_Success() => this.CreateGroup(true);
        [Test]
        [Category("CreateGroup")]
        public void CreateGroup_BadRequest() => this.CreateGroup(false);
        private void CreateGroup(bool cacheResult)
        {
            IList<Assessment.Group.Phase.Models.Match> matches = new List<Assessment.Group.Phase.Models.Match>();
            IList<TeamStats> teamStats = new List<TeamStats>()
            {
                new TeamStats(),
                new TeamStats(),
                new TeamStats(),
                new TeamStats(),
            };
            var group = new GroupEntity()
            {
                TeamStats = teamStats,
                Matches = matches
            };

            bool resultTryGetValue = cacheResult;

            _simulationService.Setup(s => s.GenerateFixture(It.IsAny<IList<TeamStats>>())).Returns(matches);
            _cacheService.Setup(c => c.TryGetValue<IList<TeamStats>>(CacheKeyEnum.TeamStats, out teamStats)).Returns(resultTryGetValue);
            _cacheService.Setup(c => c.Set<GroupEntity>(CacheKeyEnum.Group, group)).Returns(group);

            try
            {
                // Act
                var result = _sut.CreateGroup();

                // Assert
                result.Should().NotBeNull();
                result.Should().BeAssignableTo<GroupEntity>();
                result.Should().BeEquivalentTo(group);
            } 
            catch(BadRequestException ex)
            {
                ex.Should().NotBeNull();
                ex.Message.Should().NotBeNull();
            }
        }
        #endregion

        #region GetGroup
        [Test]
        [Category("GetGroup")]
        public void GetGroup_Success() => this.GetGroup(true);
        [Test]
        [Category("GetGroup")]
        public void GetGroup_BadRequest() => this.GetGroup(false);
        public void GetGroup(bool cacheResult)
        {
            GroupEntity groupEntity = new GroupEntity();
            bool tryGetValueResult = cacheResult;

            _cacheService.Setup(c => c.TryGetValue<GroupEntity>(CacheKeyEnum.Group, out groupEntity)).Returns(tryGetValueResult);

            try
            {
                // Act
                var result = _sut.GetGroup();

                // Assert
                result.Should().NotBeNull();
                result.Should().BeAssignableTo<GroupEntity>();
                result.Should().BeEquivalentTo(groupEntity);
            }
            catch (BadRequestException ex)
            {
                ex.Should().NotBeNull();
                ex.Message.Should().NotBeNull();
            }
        }
        #endregion

        #region SimulateMatchDays 
        [Test]
        [Category("SimulateMatchDays")]
        public void SimulateMatchDays_Success() => this.SimulateMatchDays(true);
        [Test]
        [Category("SimulateMatchDays")]
        public void SimulateMatchDays_BadRequest() => this.SimulateMatchDays(false);
        public void SimulateMatchDays(bool cacheResult)
        {
            IList<TeamStats> teamStats = new List<TeamStats>()
            {
                new TeamStats(),
                new TeamStats(),
                new TeamStats(),
                new TeamStats(),
            };
            var group = new GroupEntity()
            {
                TeamStats = teamStats,
                Matches = new List<Assessment.Group.Phase.Models.Match>()
                {
                    new Assessment.Group.Phase.Models.Match()
                    {
                        MatchDay = 1
                    },
                    new Assessment.Group.Phase.Models.Match()
                    {
                        MatchDay = 2
                    },
                    new Assessment.Group.Phase.Models.Match()
                    {
                        MatchDay = 3
                    }
                }
            };

            var returnTryGetValue = cacheResult;
            var matchDay = 1;
            var expectedResult = group.Matches.ToList().Where(m => m.MatchDay == matchDay);

            _cacheService.Setup(c => c.TryGetValue<GroupEntity>(CacheKeyEnum.Group, out group)).Returns(returnTryGetValue);
            _simulationService.Setup(s => s.SimulateMatch(It.IsAny<Assessment.Group.Phase.Models.Match>()));

            try
            {
                // Act
                var result = _sut.SimulateMatchDay(matchDay);
                // Assert
                result.Should().NotBeNull();
                result.Should().BeAssignableTo<IList<Assessment.Group.Phase.Models.Match>>();
                result.Should().BeEquivalentTo(expectedResult);
                result.Should().HaveCount(expectedResult.Count());

            }
            catch (BadRequestException ex)
            {
                ex.Should().NotBeNull();
                ex.Message.Should().NotBeNull();
            }
        }
        #endregion

        [Test]
        public void SaveGroup()
        {
            var groupEntity = new GroupEntity();

            SeTLogExceptionAndRollbackTransactionIfFail(groupEntity);
            _groupRepository.Setup(r => r.SaveGroup(It.IsAny<GroupEntity>())).Returns(groupEntity);

            // Act
            var result = _sut.SaveGroup(groupEntity);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<GroupEntity>();
            result.Should().BeEquivalentTo(groupEntity);
        }

        [Test]
        public void GetGroupHistory()
        {
            IList<GroupEntity> groups = new List<GroupEntity>()
            {
                new GroupEntity(),
                new GroupEntity(),
                new GroupEntity(),
            };
            var last = 10;

            SetLogExceptionIfFail(groups);

            _groupRepository.Setup(r => r.GetGroupsHistory(It.IsAny<int>())).Returns(groups);

            // Act
            var result = _sut.GetGroupsHistory(last);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<List<GroupEntity>>();
            result.Should().HaveCount(groups.ToList().Count());
            result.Should().BeEquivalentTo(groups);
        }

        #region Transaction
        private void SetLogExceptionIfFail<T>(T returnValue)
        {
            // Setup
            _repository.Setup(x => x.LogExceptionIfFail(It.IsAny<ILogger>(), It.IsAny<Func<T>>()))
                 .Returns((ILogger aLogger, Func<T> func) => new Repository().LogExceptionIfFail(aLogger, func));
        }
        private void SeTLogExceptionAndRollbackTransactionIfFail<T>(T returnValue)
        {
            // Setup
            _repository.Setup(x => x.LogExceptionAndRollbackTransactionIfFail(It.IsAny<ILogger>(), It.IsAny<Func<T>>()))
                 .Returns((ILogger aLogger, Func<T> func) => new Repository().LogExceptionAndRollbackTransactionIfFail(aLogger, func));
        }
        #endregion
    }
}
