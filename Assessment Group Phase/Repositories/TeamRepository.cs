using Assessment.Group.Phase.Models;
using System.Collections.Generic;
using System.Linq;

namespace Assessment.Group.Phase.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly Context _context;
        public TeamRepository(Context context)
        {
            _context = context;
        }
        public IList<Team> GetTeams()
        {
            return _context.Teams.ToList();
        }
        public Team SaveTeam(Team team)
        {
            _context.Teams.AddAsync(team);
            _context.SaveChanges();
            return team;
        }
    }
}
