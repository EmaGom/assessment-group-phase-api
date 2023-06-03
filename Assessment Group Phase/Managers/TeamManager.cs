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
    public class TeamManager : ITeamManager 
    {
        private readonly ILogger _logger;
        private readonly ICacheService _cacheService;
        private readonly IReaderService _readerService;
        private readonly IRepository _repository;
        private readonly ITeamRepository _teamRepository;

        public TeamManager(ILogger<TeamManager> logger,
            ICacheService cacheService,
            IReaderService readerService,
            IRepository repository,
            ITeamRepository teamRepository)
        {
            _logger = logger;   
            _cacheService = cacheService;
            _readerService = readerService;
            _repository = repository;
            _teamRepository = teamRepository;
        }

        public IList<TeamStatics> GetTeamsStatics()
        {
            if(_cacheService.TryGetValue<TeamStatics>(CacheKeyEnum.TeamStatic, out IList<TeamStatics> teamStatics))
            {
                teamStatics = teamStatics.OrderByDescending(t => t.Points)
                         .ThenByDescending(t => t.Difference)
                         .ThenByDescending(t => t.For)
                         .ThenBy(t => t.Against)
                .ToList();

                _cacheService.Update(CacheKeyEnum.Teams, teamStatics);
                return teamStatics;
            }
            throw new BadRequestException(ErrorMessages.NoTeamStaticsFound);
        }

        public IList<Team> SetTeams()
        {
            var teams = _repository.LogExceptionIfFail(_logger, () =>
            {
                return  _teamRepository.GetTeams();
            });

            var teamStatics = new List<TeamStatics>();
            if (teams.Any())
            {
                var teamsInformation = _readerService.Read();
                foreach (var team in teams)
                {
                    var teamInformation = teamsInformation.Where(t => t.Id == team.Id).FirstOrDefault();
                    team.Players = teamInformation.Players;
                    team.Offense = teamInformation.Offense;
                    team.Defence = teamInformation.Defence;
                    team.HomeStreng = teamInformation.HomeStreng;

                    teamStatics.Add(new TeamStatics()
                    {
                        Team = team,
                        TeamId = team.Id
                    });
                }
            }
            _cacheService.Set(CacheKeyEnum.Teams, teams);
            _cacheService.Set(CacheKeyEnum.TeamStatic, teamStatics);

            return teams;
        }
    }
}
