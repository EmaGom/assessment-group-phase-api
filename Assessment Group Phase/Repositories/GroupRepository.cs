using Assessment.Group.Phase.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Assessment.Group.Phase.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly Context _context;
        public GroupRepository(Context context)
        {
            _context = context;
        }

        public IList<GroupEntity> GetGroupsHistory(int last)
        {
            return _context.Groups.AsNoTracking().OrderByDescending(t => t.Id)
                                .Take(last).Include(g => g.TeamStatics).ThenInclude(ts => ts.Team).ToList();
        }

        public GroupEntity SaveGroup(GroupEntity group)
        {
            foreach (var teamStatic in group.TeamStatics)
                _context.Entry(teamStatic.Team).State = EntityState.Unchanged;

            _context.Groups.Add(group);
            _context.SaveChanges();
            return group;
        }
    }
}
