using HealtChecker.Service.Metrics.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Data.Interfaces
{
    public interface IMetricsDbContext
    {
        DbSet<Metric> Metrics { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
