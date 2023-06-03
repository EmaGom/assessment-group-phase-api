using Assessment.Group.Phase.Models;
using System.Collections.Generic;

namespace Assessment.Group.Phase.Repositories
{
    public interface IGroupRepository
    {
        IList<GroupEntity> GetGroupsHistory(int last);
        GroupEntity SaveGroup(GroupEntity group);
    }
}
