using CA.Application.Common.Interface;
using CA.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;


namespace CA.Persistance
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        // This constructor is used of runit testing
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                .UseSqlServer("DataSource=app.db");
            }

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent API configurations 
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            //modelBuilder.Seed();

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditLogEntry>())
            {
                switch (entry.State)
                {
                    //case EntityState.Added:
                    //    entry.Entity.CreatedBy = _currentUserService.UserId;
                    //    entry.Entity.Created = _dateTime.Now;
                    //    break;
                    //case EntityState.Modified:
                    //    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    //    entry.Entity.LastModified = _dateTime.Now;
                    //    break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
