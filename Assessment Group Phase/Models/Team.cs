using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment.Group.Phase.Models
{
    [Table("Team")]
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string Name {  get; set; }
        public double? Offense { get; set; }
        public double? Defence { get; set; }
        public double? HomeStreng { get; set; }
        public IList<TeamStatics> TeamStatics { get; set; }
        [NotMapped]
        public double Overall => (Offense.GetValueOrDefault() + Defence.GetValueOrDefault()) / 2;
        [NotMapped]
        public double SimulationStrengh => (Overall / 2) / 10;
        [NotMapped]
        public IList<Player> Players { get; set; }
    }
}
