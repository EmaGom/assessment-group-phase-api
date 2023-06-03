using Assessment.Group.Phase.Enums;
using Assessment.Group.Phase.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Assessment.Group.Phase.Helpers
{
    public class SimulationServices : ISimulationService
    {
        public IList<Match> GenerateFixture(IList<TeamStatics> teamStatics)
        {
            // Improve this..
            var fixture = new List<Match>();

            fixture.Add(new Match()
            {
                HomeTeamStatics = teamStatics[0],
                AwayTeamStatics = teamStatics[1],
                MatchDay = 1
            });

            fixture.Add(new Match()
            {
                HomeTeamStatics = teamStatics[2],
                AwayTeamStatics = teamStatics[3],
                MatchDay = 1
            });

            fixture.Add(new Match()
            {
                HomeTeamStatics = teamStatics[1],
                AwayTeamStatics = teamStatics[2],
                MatchDay = 2
            });

            fixture.Add(new Match()
            {
                HomeTeamStatics = teamStatics[0],
                AwayTeamStatics = teamStatics[3],
                MatchDay = 2
            });

            fixture.Add(new Match()
            {
                HomeTeamStatics = teamStatics[0],
                AwayTeamStatics = teamStatics[2],
                MatchDay = 3
            });

            fixture.Add(new Match()
            {
                HomeTeamStatics = teamStatics[1],
                AwayTeamStatics = teamStatics[3],
                MatchDay = 3
            });
            return fixture;
        }
        private void RotateTeams(IList<TeamStatics> teamStatics)
        {
            // Rotate teams by moving the last team to the second position
            TeamStatics lastTeam = teamStatics[teamStatics.Count - 1];
            teamStatics.RemoveAt(teamStatics.Count - 1);
            teamStatics.Insert(1, lastTeam);
        }
        public void SimulateMatch(Match match)
        {
            var random = new Random();

            match.HomeScore = random.Next(0, Convert.ToInt32(Math.Round(match.HomeTeamStatics.Team.SimulationStrengh + match.HomeTeamStatics.Team.HomeStreng.GetValueOrDefault())));
            match.AwayScore = random.Next(0, Convert.ToInt32(Math.Round(match.AwayTeamStatics.Team.SimulationStrengh)));

            UpdateStatics(match);
        }
        public IList<Player> GetScorers(GroupEntity group)
        {
            var scorers = new List<Player>();
            foreach (var teamStatics in group.TeamStatics)
            {
                var scorersTeam = teamStatics.Team.Players.Where(p => p.Goals > 0).ToList();
                scorers.AddRange(scorersTeam);
            }

            return scorers.OrderByDescending(s => s.Goals).Take(10).ToList();
        }
        public IList<Player> GetAssistans(GroupEntity group)
        {
            var scorers = new List<Player>();
            foreach (var teamStatics in group.TeamStatics)
            {
                var scorersTeam = teamStatics.Team.Players.Where(p => p.Assists > 0).ToList();
                scorers.AddRange(scorersTeam);
            }

            return scorers.OrderByDescending(s => s.Assists).Take(10).ToList();
        }
        private void UpdateStatics(Match match)
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

            UpdateTeamStatics(match.HomeTeamStatics, homeResult, match.HomeScore, match.AwayScore);
            UpdateTeamStatics(match.AwayTeamStatics, awayResult, match.AwayScore, match.HomeScore);
        }

        private void UpdateTeamStatics(TeamStatics teamStatics, ResultPointsEnum result, int forGoals, int againstGoals)
        {
            teamStatics.Points = ((int)result);

            switch (result)
            {
                case ResultPointsEnum.Win:
                    teamStatics.Win += 1;
                    break;
                case ResultPointsEnum.Draw:
                    teamStatics.Draw += 1;
                    break;
                case ResultPointsEnum.Loss:
                    teamStatics.Loss += 1;
                    break;

            }

            teamStatics.For += forGoals;
            teamStatics.Against -= againstGoals;
            teamStatics.Difference = teamStatics.For + teamStatics.Against;
            teamStatics.Played += 1;

            if(forGoals > 0)
                UpdatePlayerStatics(teamStatics.Team, forGoals);
        }
        private void UpdatePlayerStatics(Team team, int goals)
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
