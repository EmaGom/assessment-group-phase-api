using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment.Group.Phase.Models
{
    [Table("Group")]
    public class GroupEntity
    {
        [Key]
        public int Id { get; set; }
        public IList<TeamStats> TeamStats { get; set; }
        public IList<Match> Matches { get; set; }
        public IList<Player> Scorers { get; set; }
        public IList<Player> Assistants { get; set; }
    }
}
