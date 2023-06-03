using Assessment.Group.Phase.Models;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Group.Phase.Repositories
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
           : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().HasKey(t => t.Id).HasName("Id");
            modelBuilder.Entity<TeamStatics>().HasKey(t => t.Id).HasName("Id");
            modelBuilder.Entity<GroupEntity>().HasKey(t => t.Id).HasName("Id");

            modelBuilder.Entity<TeamStatics>().HasOne(ts => ts.Team).WithMany(t => t.TeamStatics)
                .HasForeignKey(ts => ts.TeamId);
            modelBuilder.Entity<TeamStatics>().HasOne(ts => ts.Group).WithMany(t => t.TeamStatics)
                .HasForeignKey(ts => ts.GroupId);

    }
        public virtual DbSet<GroupEntity> Groups { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamStatics> TeamsStatics { get; set; }
    }
}
