using Assessment.Group.Phase.Enums;
using Assessment.Group.Phase.Helpers;
using Assessment.Group.Phase.Models;
using Assessment.Group.Phase.Models.Exceptions;
using Assessment.Group.Phase.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Assessment.Group.Phase.Managers
{
    public class GroupManager : IGroupManager
    {
        private readonly ILogger _logger;         
        private readonly ICacheService _cacheService;
        private readonly ITeamManager _teamManager;
        private readonly ISimulationService _simulationService;
        private readonly IRepository _repository;
        private readonly IGroupRepository _groupRepository;

        public GroupManager(ILogger<GroupManager> logger,
            ICacheService cacheService,
            ITeamManager teamManager,
            ISimulationService simulationService,
            IRepository repository,
            IGroupRepository groupRepository)
        {
            _logger = logger;
            _cacheService = cacheService;
            _teamManager = teamManager;
            _simulationService = simulationService;
            _repository = repository;
            _groupRepository = groupRepository;
        }

        public GroupEntity CreateGroup()
        {
            if (_cacheService.TryGetValue<IList<TeamStatics>>(CacheKeyEnum.TeamStatic, out IList<TeamStatics> teamStatics))
            {
                var group = new GroupEntity()
                {
                    TeamStatics = teamStatics,
                    Matches = _simulationService.GenerateFixture(teamStatics)
                };
                _cacheService.Set<GroupEntity>(CacheKeyEnum.Group, group);
                return group;
            } else
            {
                throw new BadRequestException(ErrorMessages.NoTeamStaticsFound);
            }

        }

        public GroupEntity GetGroup()
        {
            if (_cacheService.TryGetValue<GroupEntity>(CacheKeyEnum.Group, out GroupEntity group))
            {
                group.TeamStatics = _teamManager.GetTeamsStatics();
                group.Scorers = _simulationService.GetScorers(group);
                group.Assistants = _simulationService.GetAssistans(group);
                return group;
            }
            throw new BadRequestException(ErrorMessages.NoGroupFound);
        }

        public IList<Match> SimulateMatchDay(int matchDay)
        {
            if (_cacheService.TryGetValue<GroupEntity>(CacheKeyEnum.Group, out GroupEntity group))
            {
                var matches = group.Matches.Where(m => m.MatchDay == matchDay).ToList();

                foreach (var match in matches)
                {
                    _simulationService.SimulateMatch(match);
                }
                return matches;
            }
            throw new BadRequestException(ErrorMessages.NoMatchesFound);
        }

        public GroupEntity SaveGroup(GroupEntity group)
        {
            return _repository.LogExceptionAndRollbackTransactionIfFail(_logger, () =>
            {
                return _groupRepository.SaveGroup(group);
            });
        }
        public IList<GroupEntity> GetGroupsHistory(int last)
        {
            return _repository.LogExceptionIfFail(_logger, () =>
            {
                return _groupRepository.GetGroupsHistory(last);
            });
        }
    }
}
