using Assessment.Group.Phase.Helpers;
using Assessment.Group.Phase.Managers;
using Assessment.Group.Phase.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Assessment.Group.Phase
{
    public static class ServicesConfiguration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<ILoggerFactory, LoggerFactory>();
            services.AddTransient<IGroupManager, GroupManager>();
            services.AddTransient<ITeamManager, TeamManager>();
            services.AddTransient<IReaderService, ReaderService>();
            services.AddTransient<ISimulationService, SimulationServices>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
        }
    }
}
