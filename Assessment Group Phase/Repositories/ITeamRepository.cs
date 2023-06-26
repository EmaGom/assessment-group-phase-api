using Assessment.Group.Phase.Models;
using System.Collections.Generic;

namespace Assessment.Group.Phase.Repositories
{
    public interface ITeamRepository
    {
        IList<Team> GetTeams();
        Team SaveTeam(Team team);
    }
}
