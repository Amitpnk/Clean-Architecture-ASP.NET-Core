using CleanArch.Domain.Entities;
using CleanArch.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Persistence.Context;

public class AuditableContext(DbContextOptions options) : IdentityContext(options)
{
    public DbSet<AuditTrail> AuditTrails { get; set; }

    /// <summary>
    /// Saves changes and writes <see cref="AuditTrail"/> rows attributed to <paramref name="userId"/>.
    /// Deliberately not an overload of SaveChangesAsync: a parameterless <c>SaveChangesAsync()</c> call
    /// used to bind to it and bypass the audit-column stamping in ApplicationDbContext.
    /// </summary>
    protected async Task<int> SaveChangesWithAuditAsync(string userId, CancellationToken cancellationToken = default)
    {
        var auditEntries = OnBeforeSaveChanges(userId);
        var result = await base.SaveChangesAsync(cancellationToken);
        await OnAfterSaveChanges(auditEntries, cancellationToken);
        return result;
    }

    private List<AuditEntry> OnBeforeSaveChanges(string userId)
    {
        ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditTrail || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            var auditEntry = new AuditEntry(entry);
            auditEntry.TableName = entry.Entity.GetType().Name;
            auditEntry.UserId = userId;
            auditEntries.Add(auditEntry);
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    auditEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.AuditType = AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = AuditType.Update;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }
        foreach (var auditEntry in auditEntries.Where(e => !e.HasTemporaryProperties))
        {
            AuditTrails.Add(auditEntry.ToAudit());
        }
        return auditEntries.Where(e => e.HasTemporaryProperties).ToList();
    }

    private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken)
    {
        if (auditEntries == null || auditEntries.Count == 0)
            return Task.CompletedTask;

        foreach (var auditEntry in auditEntries)
        {
            foreach (var prop in auditEntry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }
            AuditTrails.Add(auditEntry.ToAudit());
        }
        // Persist only the completed audit rows; don't audit the audit rows themselves.
        return base.SaveChangesAsync(cancellationToken);
    }
}