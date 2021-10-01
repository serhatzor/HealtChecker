using HealtChecker.Service.Logging.Data.Entities;
using HealtChecker.Service.Logging.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealtChecker.Service.Logging.Data.Implementations
{
    public class LogDbContext : DbContext, ILogDbContext
    {
        public DbSet<Log> Logs { get; set; }

        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
