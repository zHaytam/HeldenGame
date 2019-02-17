using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeldenServer.Database.Models;
using HeldenServer.Database.Properties;
using Microsoft.EntityFrameworkCore;

namespace HeldenServer.Database
{
    public class DatabaseContext : DbContext
    {

        #region Properties

        public DbSet<User> Users { get; set; }

        #endregion

        public DatabaseContext() : base(GetOptions()) { }

        #region Public Methods

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            AddTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        #endregion

        #region Private Methods

        private static DbContextOptions GetOptions() => new DbContextOptionsBuilder().UseMySql(Resources.ConnectinString).Options;

        private void AddTimestamps()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.DateCreated = DateTime.UtcNow;
                }

                entity.DateModified = DateTime.UtcNow;
            }
        }

        #endregion

    }
}
