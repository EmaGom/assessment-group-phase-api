using Assessment.Group.Phase.Models;
using System.Collections.Generic;

namespace Assessment.Group.Phase.Managers
{
    public interface IGroupManager
    {
        /// <summary>
        /// Check if exists a group in cache and return it with the Team Stats and the Scorers
        /// </summary>
        /// <returns>Group</returns>
        GroupEntity GetGroup();

        /// <summary>
        /// Create a new Group and set into the Cache Service, return the new group created
        /// </summary>
        /// <returns>Group created</returns>
        GroupEntity CreateGroup();

        /// <summary>
        /// Simulate a match day
        /// </summary>
        /// <param name="matchDay">The matchday number</param>
        /// <returns>List of the simulated matchs</returns>
        IList<Match> SimulateMatchDay(int matchDay);

        /// <summary>
        /// Save a group into the database, with its team stats related
        /// </summary>
        /// <param name="group">The group to be saved</param>
        /// <returns>Group updated with Id</returns>
        GroupEntity SaveGroup(GroupEntity group);

        /// <summary>
        /// Get the last groups saved
        /// </summary>
        /// <param name="last">the number of the last groups to get</param>
        /// <returns>A List of Groups</returns>
        IList<GroupEntity> GetGroupsHistory(int last);
    }
}
