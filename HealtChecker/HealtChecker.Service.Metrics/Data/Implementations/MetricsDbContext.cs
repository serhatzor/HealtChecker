using HealtChecker.Service.Metrics.Data.Entities;
using HealtChecker.Service.Metrics.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealtChecker.Service.Metrics.Data.Implementations
{
    public class MetricsDbContext : DbContext, IMetricsDbContext
    {
        public DbSet<Metric> Metrics { get; set; }

        public MetricsDbContext(DbContextOptions<MetricsDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            InitializeBaseEntity<Metric>(modelBuilder);
        }

        private void InitializeBaseEntity<T>(ModelBuilder modelBuilder) where T : BaseEntity
        {
            modelBuilder.Entity<T>().HasKey(x => x.Id);

            modelBuilder.Entity<T>().Property(x => x.CreatedAt)
                .IsRequired(true);
        }

        public async Task<int> SaveChangesAsync()
        {
            SetCommonFields();
            return await base.SaveChangesAsync();
        }

        public override int SaveChanges()
        {
            SetCommonFields();
            return base.SaveChanges();
        }

        private void SetCommonFields()
        {
            ChangeTracker.DetectChanges();
            var markedAsAdded = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);

            foreach (var item in markedAsAdded)
            {
                if (item.Entity is BaseEntity entity)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.Id = Guid.NewGuid();
                }
            }
        }
    }
}
