using Assessment.Group.Phase.Models;
using System.Collections.Generic;

namespace Assessment.Group.Phase.Helpers
{
    public interface IReaderService
    {
         IList<Team> Read();
    }
}
