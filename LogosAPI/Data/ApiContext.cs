
using LogosAPI.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace LogosAPI.Data
{
    public class ApiContext : DbContext
    {

        public DbSet<issues> issues { get; set; }
        public DbSet<plannedmaintenance> plannedmaintenance { get; set; }

        public DbSet<User> users { get; set; }

        protected readonly IConfiguration _configuration;

        public ApiContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to mysql with connection string from app settings
            var connectionString = _configuration.GetConnectionString("LogosDataBase");
            options.UseMySql(connectionString, MariaDbServerVersion.LatestSupportedServerVersion, options => options.EnableRetryOnFailure());
        }

    }
}
