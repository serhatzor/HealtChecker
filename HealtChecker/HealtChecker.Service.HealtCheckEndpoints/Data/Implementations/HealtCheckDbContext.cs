using HealtChecker.Service.HealtCheckEndpoints.Data.Entities;
using HealtChecker.Service.HealtCheckEndpoints.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Data.Implementations
{
    public class HealtCheckDbContext : DbContext, IHealtCheckDbContext
    {
        public DbSet<HealtCheckEnpoint> HealtCheckEnpoints { get; set; }

        public HealtCheckDbContext(DbContextOptions<HealtCheckDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HealtCheckEnpoint>().Property(x => x.Name)
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired(true);

            modelBuilder.Entity<HealtCheckEnpoint>().Property(x => x.HealtCheckUrl)
                .HasMaxLength(2048)
                .IsUnicode(true)
                .IsRequired(true);

            modelBuilder.Entity<HealtCheckEnpoint>().Property(x => x.ConnectedUserId)
                .IsRequired(true);

            modelBuilder.Entity<HealtCheckEnpoint>().Property(x => x.IntervalSeconds)
                .IsRequired(true);

            modelBuilder.Entity<HealtCheckEnpoint>().Property(x => x.NextExecutionTime)
                .IsRequired(true);

            modelBuilder.Entity<HealtCheckEnpoint>().Property(x => x.NotificationEmailAddress)
                .HasMaxLength(512)
                .IsUnicode(true)
                .IsRequired(true);



            modelBuilder.Entity<HealtCheckEnpoint>()
                .HasIndex(x => new { x.ConnectedUserId, x.HealtCheckUrl })
                .IsUnique(true);

            InitializeBaseEntity<HealtCheckEnpoint>(modelBuilder);
        }

        private void InitializeBaseEntity<T>(ModelBuilder modelBuilder) where T : BaseEntity
        {
            modelBuilder.Entity<T>().HasKey(x => x.Id);

            modelBuilder.Entity<T>().Property(x => x.CreatedAt)
                .IsRequired(true);

            modelBuilder.Entity<T>().Property(x => x.CreatedUserId)
                .IsRequired(true);

            modelBuilder.Entity<T>().Property(x => x.UpdatedAt)
                .IsRequired(false);

            modelBuilder.Entity<T>().Property(x => x.UpdatedUserId)
                .IsRequired(false);
        }

        public async Task<int> SaveChangesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            SetCommonFields(userId);
            return await base.SaveChangesAsync(cancellationToken);
        }

        public int SaveChanges(Guid userId)
        {
            SetCommonFields(userId);
            return base.SaveChanges();
        }

        private void SetCommonFields(Guid userId)
        {
            ChangeTracker.DetectChanges();
            var markedAsUpdated = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified);
            var markedAsAdded = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);

            foreach (var item in markedAsUpdated)
            {
                if (item.Entity is BaseEntity entity)
                {
                    entity.UpdatedAt = DateTime.UtcNow;
                    if (userId != Guid.Empty)
                        entity.UpdatedUserId = userId;
                }

            }
            foreach (var item in markedAsAdded)
            {
                if (item.Entity is BaseEntity entity)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                    entity.CreatedUserId = userId;
                    entity.Id = Guid.NewGuid();
                }
            }
        }


    }
}
