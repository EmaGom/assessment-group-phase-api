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
            modelBuilder.Entity<TeamStats>().HasKey(t => t.Id).HasName("Id");
            modelBuilder.Entity<GroupEntity>().HasKey(t => t.Id).HasName("Id");

            modelBuilder.Entity<TeamStats>().HasOne(ts => ts.Team).WithMany(t => t.TeamStats)
                .HasForeignKey(ts => ts.TeamId);
            modelBuilder.Entity<TeamStats>().HasOne(ts => ts.Group).WithMany(t => t.TeamStats)
                .HasForeignKey(ts => ts.GroupId);

    }
        public virtual DbSet<GroupEntity> Groups { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamStats> TeamsStats { get; set; }
    }
}
