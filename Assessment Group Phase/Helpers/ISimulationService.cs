using Assessment.Group.Phase.Models;
using System.Collections.Generic;

namespace Assessment.Group.Phase.Helpers
{
    public interface ISimulationService
    {
        IList<Match> GenerateFixture(IList<TeamStats> teamStats);
        void SimulateMatch(Match match);
        IList<Player> GetScorers(GroupEntity group);
        IList<Player> GetAssistans(GroupEntity group);
    }
}
