using HealtChecker.Service.Logging.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealtChecker.Service.Logging.Data.Interfaces
{
    public interface ILogDbContext
    {
        DbSet<Log> Logs { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}