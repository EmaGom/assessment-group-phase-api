using Assessment.Group.Phase.Enums;
using Assessment.Group.Phase.Helpers;
using Assessment.Group.Phase.Managers;
using Assessment.Group.Phase.Models.Exceptions;
using Assessment.Group.Phase.Models;
using Assessment.Group.Phase.Repositories;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;

namespace Assassment.Group.Phase.Test.ManagersTests
{
    public class TeamManagerTest
    {
        private Mock<ILogger<TeamManager>> _logger;
        private Mock<IReaderService> _readerService;
        private Mock<ICacheService> _cacheService;
        private Mock<IRepository> _repository;
        private Mock<ITeamRepository> _teamRepository;
        private TeamManager _sut;

        [SetUp]
        public void Init()
        {
            _logger = new Mock<ILogger<TeamManager>>();
            _cacheService = new Mock<ICacheService>();
            _readerService = new Mock<IReaderService>();
            _repository = new Mock<IRepository>();
            _teamRepository = new Mock<ITeamRepository>();

            _sut = new TeamManager(_logger.Object, _cacheService.Object, _readerService.Object, _repository.Object, _teamRepository.Object);
        }

        [TearDown]
        public void destroy()
        {
            this._cacheService = null;
            this._readerService = null;
            this._teamRepository = null;
            this._sut = null;
        }

        #region GetTeamsStats
        [Test]
        [Category("GetTeamsStats")]
        public void GetTeamsStats_Success() => this.GetTeamsStats(true);
        [Test]
        [Category("GetTeamsStats")]
        public void GetTeamsStats_BadRequest() => this.GetTeamsStats(false);
        private void GetTeamsStats(bool cacheResult)
        {
            int[] expectedPositionsId = { 1, 2, 3, 4 };
            IList<TeamStats> teamStats = new List<TeamStats>()
            {
                new TeamStats(){
                    Id = 3,
                    Points = 7,
                },
                new TeamStats(){
                    Id = 2,
                    Points = 9,
                    Difference = 2,
                    For = 2,
                    Against = 0
                },
                new TeamStats(){
                    Id = 4,
                    Points = 6,
                },
                new TeamStats()
                {
                    Id = 1,
                    Points = 9,
                    Difference = 2,
                    For = 4,
                    Against = 2
                },
            };

            bool resultTryGetValue = cacheResult;

            _cacheService.Setup(c => c.TryGetValue<TeamStats>(CacheKeyEnum.TeamStats, out teamStats)).Returns(resultTryGetValue);
            _cacheService.Setup(c => c.Update(CacheKeyEnum.Teams, teamStats));

            try
            {
                // Act
                var result = _sut.GetTeamsStats();

                // Assert
                result.Should().NotBeNull();
                result.Should().BeAssignableTo<List<TeamStats>>();
                result.Should().BeEquivalentTo(teamStats);
                result.Should().HaveCount(teamStats.Count());

                foreach (var idPos in expectedPositionsId)
                    result[idPos - 1].Id.Should().Be(idPos);
            }
            catch (BadRequestException ex)
            {
                ex.Should().NotBeNull();
                ex.Message.Should().NotBeNull();
            }
        }
        #endregion

        [Test]
        public void SetTeams()
        {
            IList<Team> teams = new List<Team>() 
            { 
                new Team()
                {
                    Id = 1,
                    Name = "Team1"
                }
            };
            var readerTeams = new List<Team>()
            {
                new Team()
                {
                    Id = 1,
                    Name = "Team1",
                    Offense = 90,
                    Defence = 80,
                    Players = new List<Player>()
                }
            };
            var team = new Team();
            var teamStats = new TeamStats();

            SetLogExceptionIfFail(teams);
            _cacheService.Setup(c => c.Set(CacheKeyEnum.Teams, team));
            _cacheService.Setup(c => c.Set(CacheKeyEnum.TeamStats, teamStats));
            _readerService.Setup(r => r.Read()).Returns(teams);

            _teamRepository.Setup(r => r.GetTeams()).Returns(teams);

            // Act
            var result = _sut.SetTeams();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<List<Team>>();
            result.Should().BeEquivalentTo(teams);
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
