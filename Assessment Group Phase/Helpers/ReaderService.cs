using Assessment.Group.Phase.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Assessment.Group.Phase.Helpers
{
    public class ReaderService : IReaderService
    {
        private const string FileName = "teams.json";
        public IList<Team> Read()
        {
            using (var reader = new StreamReader(FileName))
            {
                var json = reader.ReadToEnd();
                IList<Team> teams = JsonConvert.DeserializeObject<List<Team>>(json);
                return teams;
            }
        }
    }
}
