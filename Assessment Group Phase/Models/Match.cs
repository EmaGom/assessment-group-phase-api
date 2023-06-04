using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace Assessment.Group.Phase.Models
{
    [NotMapped]
    public class Match
    {
        [Key]
        public int Id { get; set; }
        public TeamStats HomeTeamStats { get; set; }  
        public TeamStats AwayTeamStats { get; set; }  
        public int MatchDay { get; set; } 
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}
