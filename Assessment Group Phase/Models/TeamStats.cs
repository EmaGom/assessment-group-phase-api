using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment.Group.Phase.Models
{
    [Table("TeamStats")]
    public class TeamStats
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int TeamId { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }
        public int Played { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Loss { get; set; }
        public int For { get; set; }
        public int Against { get; set; }
        public int Difference { get; set; }
        public Team Team { get; set; }
        public GroupEntity Group { get; set; }
    }
}
