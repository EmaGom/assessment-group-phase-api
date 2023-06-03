using Assessment.Group.Phase.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment.Group.Phase.Models
{
    [NotMapped]
    public class Player
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } 
        public PositionEnum Position { get; set; }
        public int Overall { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public double GoalProbability { get; set; }
        public double AssistProbability { get; set; }

    }
}
