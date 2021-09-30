using HealtChecker.Service.HealtCheckEndpoints.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Data.Interfaces
{
    public interface IHealtCheckDbContext
    {
        DbSet<HealtCheckEnpoint> HealtCheckEnpoints { get; set; }
        int SaveChanges(Guid userId);
        Task<int> SaveChangesAsync(Guid userId,CancellationToken cancellationToken = default);
    }
}
