using Microsoft.EntityFrameworkCore;
using pagSeguro.Api.Entities;

namespace pagSeguro.Api.Helpers
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Log> Logs { get; set; }
    }
}
