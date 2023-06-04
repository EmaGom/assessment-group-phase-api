using Assessment.Group.Phase.Enums;
using Assessment.Group.Phase.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Assessment.Group.Phase.Models.Exceptions;

namespace Assessment.Group.Phase.Helpers
{
    public class SimulationServices : ISimulationService
    {
        public IList<Match> GenerateFixture(IList<TeamStats> teamStats)
        {
            IList<Match> fixture = new List<Match>();
            int rounds = teamStats.Count() - 1;

            //Randomly order teams to change home strength in each simulation
            var random = new Random();
            teamStats = teamStats.OrderBy(t => random.Next()).ToList();
            for (int homeTeamIndex = 0; homeTeamIndex < teamStats.Count(); homeTeamIndex++)
            {
                var matchDay = 1;
                var homeTeam = teamStats[homeTeamIndex];
                for(int awayTeamIndex = homeTeamIndex + 1; awayTeamIndex < teamStats.Count(); awayTeamIndex++)
                {
                    var awayTeam = teamStats[awayTeamIndex];
                    fixture.Add(GetMatch(fixture, matchDay, rounds, homeTeam, awayTeam));
                    matchDay++;
                }
            }
            return fixture;
        }

        public void SimulateMatch(Match match)
        {
            var random = new Random();

            match.HomeScore = random.Next(0, Convert.ToInt32(Math.Round(match.HomeTeamStats.Team.SimulationStrengh + match.HomeTeamStats.Team.HomeStreng.GetValueOrDefault())));
            match.AwayScore = random.Next(0, Convert.ToInt32(Math.Round(match.AwayTeamStats.Team.SimulationStrengh)));

            UpdateStats(match);
        }
        public IList<Player> GetScorers(GroupEntity group)
        {
            var scorers = new List<Player>();
            foreach (var teamStat in group.TeamStats)
            {
                var scorersTeam = teamStat.Team.Players.Where(p => p.Goals > 0).ToList();
                foreach (var player in scorersTeam)
                    player.Team = teamStat.Team.Name;

                scorers.AddRange(scorersTeam);
            }

            return scorers.OrderByDescending(s => s.Goals).Take(10).ToList();
        }
        public IList<Player> GetAssistans(GroupEntity group)
        {
            var scorers = new List<Player>();
            foreach (var teamStat in group.TeamStats)
            {
                var assistantsTeam = teamStat.Team.Players.Where(p => p.Assists > 0).ToList();
                foreach (var player in assistantsTeam)
                    player.Team = teamStat.Team.Name;
                scorers.AddRange(assistantsTeam);
            }

            return scorers.OrderByDescending(s => s.Assists).Take(10).ToList();
        }

        private Match GetMatch(IList<Match> fixture, int matchDay, int rounds, TeamStats homeTeam, TeamStats awayTeam)
        {
            while (matchDay <= rounds)
            {
                if (AreTeamsAvailablesForMatchDay(fixture, homeTeam, awayTeam, matchDay))
                {
                    return new Match()
                    {
                        AwayTeamStats = awayTeam,
                        HomeTeamStats = homeTeam,
                        MatchDay = matchDay
                    };
                }
                matchDay++;
            }
            throw new BadRequestException(ErrorMessages.NoMatchDayFound);
        }
        private bool AreTeamsAvailablesForMatchDay(IList<Match> fixture, TeamStats teamHome, TeamStats teamAway, int matchDay)
        {

            var teamHomeAvailable = fixture.Where(f => f.MatchDay == matchDay &&
                                  (f.HomeTeamStats == teamHome || f.AwayTeamStats == teamHome)).Count() > 0 ? false : true;
            var teamAwayAvailable = fixture.Where(f => f.MatchDay == matchDay &&
                                  (f.HomeTeamStats == teamAway || f.AwayTeamStats == teamAway)).Count() > 0 ? false : true;

            return teamHomeAvailable && teamAwayAvailable;
        }
        private void UpdateStats(Match match)
        {
            ResultPointsEnum homeResult, awayResult;
            if (match.HomeScore > match.AwayScore)
            {
                homeResult = ResultPointsEnum.Win;
                awayResult = ResultPointsEnum.Loss;
            }
            else if (match.HomeScore == match.AwayScore)
            {
                homeResult = ResultPointsEnum.Draw;
                awayResult = ResultPointsEnum.Draw;
            }
            else
            {
                homeResult = ResultPointsEnum.Loss;
                awayResult = ResultPointsEnum.Win;

            }

            UpdateTeamStats(match.HomeTeamStats, homeResult, match.HomeScore, match.AwayScore);
            UpdateTeamStats(match.AwayTeamStats, awayResult, match.AwayScore, match.HomeScore);
        }
        private void UpdateTeamStats(TeamStats teamStats, ResultPointsEnum result, int forGoals, int againstGoals)
        {
            teamStats.Points += ((int)result);

            switch (result)
            {
                case ResultPointsEnum.Win:
                    teamStats.Win += 1;
                    break;
                case ResultPointsEnum.Draw:
                    teamStats.Draw += 1;
                    break;
                case ResultPointsEnum.Loss:
                    teamStats.Loss += 1;
                    break;

            }

            teamStats.For += forGoals;
            teamStats.Against -= againstGoals;
            teamStats.Difference = teamStats.For + teamStats.Against;
            teamStats.Played += 1;

            if(forGoals > 0)
                UpdatePlayerStats(teamStats.Team, forGoals);
        }
        private void UpdatePlayerStats(Team team, int goals)
        {
            for (int i = 0; i < goals; i++)
            {
                var random = new Random();
                var value = random.NextDouble() * 100;
                // Goal
                var closestPlayerProbability = 100.0;
                var closestPlayer = team.Players[0];
                foreach (var player in team.Players)
                {
                    var newProbability = Math.Abs(value - player.GoalProbability);
                    if (newProbability < closestPlayerProbability)
                    {
                        closestPlayerProbability = newProbability;
                        closestPlayer = player;
                    }
                }
                closestPlayer.Goals += 1;

                // Assist
                var assistants = team.Players.Where(p => p != closestPlayer).ToList();
                value = random.NextDouble() * 100;

                closestPlayerProbability = 100.0;
                closestPlayer = assistants[0];
                foreach (var player in assistants)
                {
                    var newProbability = Math.Abs(value - player.AssistProbability);
                    if (newProbability < closestPlayerProbability)
                    {
                        closestPlayerProbability = newProbability;
                        closestPlayer = player;
                    }
                }
                closestPlayer.Assists += 1;
            }
        }
    }
}
