﻿using Assessment.Group.Phase.Models;
using System.Collections.Generic;

namespace Assessment.Group.Phase.Managers
{
    public interface ITeamManager
    {
        /// <summary>
        /// Check if exists teams in the database and updates with the json information for the simulation.
        /// </summary>
        /// <returns>List of the updated teams</returns>
        IList<Team> SetTeams();

        /// <summary>
        /// Check the teams stats saved in the cache services and order by position criteria
        /// </summary>
        /// <returns>List of team stats</returns>
        IList<TeamStats> GetTeamsStats(); 
    }
}
