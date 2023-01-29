using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DbLogger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityEntryLog;

public abstract class RepositoryBase<TEntity>
{
    private readonly DbContext _ctx;

    /// <inheritdoc />
      public async Task SaveChangesAsync(Guid userId, CancellationToken cancellationToken)
      {
         try
         {
            _ = _ctx.Set<LogEntry>();
         }
         catch (Exception e)
         {
            throw new Exception($"No Log Table configured. Message: {e.Message}");
         }


         var date = DateTime.UtcNow;
         var list = new Collection<LogEntry>();
         foreach (var entityEntry in _ctx.ChangeTracker.Entries())
         {
            var objectId = (Guid)(entityEntry.CurrentValues["Id"] ?? Guid.Empty);

            switch (entityEntry.State)
            {
               case EntityState.Deleted:
                  {
                     await GetDeletedEntryProperties(userId,
                                                     objectId,
                                                     entityEntry,
                                                     list,
                                                     date,
                                                     cancellationToken);
                     break;
                  }
               case EntityState.Added:
                  {
                     await GetAddedEntryProperties(userId,
                                                   objectId,
                                                   entityEntry,
                                                   list,
                                                   date,
                                                   cancellationToken);
                     break;
                  }
               case EntityState.Modified:
                  {
                     await GetModifiedEntryProperties(userId,
                                                      objectId,
                                                      entityEntry,
                                                      list,
                                                      date,
                                                      cancellationToken);
                     break;
                  }
               case EntityState.Detached:
               case EntityState.Unchanged:
               default:
                  continue;
            }
         }

         _ctx.AddRange(list);
         await _ctx.SaveChangesAsync(cancellationToken);
      }

      private async Task GetModifiedEntryProperties(
         Guid userId,
         Guid objectId,
         EntityEntry entityEntry,
         ICollection<LogEntry> list,
         DateTime date,
         CancellationToken cancellationToken
      )
      {
         foreach (var propertyName in entityEntry.Properties.Where(p => p.IsModified))
         {
            var context = entityEntry.Entity.GetType().Name;
            var property = propertyName.Metadata.Name;

            list.Add(LogEntry.Create(objectId,
                                     context,
                                     property,
                                     entityEntry.OriginalValues[propertyName.Metadata.Name]?.ToString(),
                                     entityEntry.CurrentValues[propertyName.Metadata.Name]?.ToString(),
                                     date,
                                     userId,
                                     LogType.Modified,
                                     await GetRevision(context, property, cancellationToken)
                                    ));
         }
      }

      private async Task GetAddedEntryProperties(
         Guid userId,
         Guid objectId,
         EntityEntry entityEntry,
         ICollection<LogEntry> list,
         DateTime date,
         CancellationToken cancellationToken
      )
      {
         foreach (var propertyName in entityEntry.Properties)
         {
            var context = entityEntry.Entity.GetType().Name;
            var property = propertyName.Metadata.Name;

            var currentValue = entityEntry.CurrentValues[propertyName.Metadata.Name]?.ToString();

            if (currentValue is null)
            {
               return;
            }

            list.Add(LogEntry.Create(objectId,
                                     entityEntry.Entity.GetType().Name,
                                     propertyName.Metadata.Name,
                                     null,
                                     currentValue,
                                     date,
                                     userId,
                                     LogType.Added,
                                     await GetRevision(context, property, cancellationToken)));
         }
      }

      private async Task GetDeletedEntryProperties(
         Guid userId,
         Guid objectId,
         EntityEntry entityEntry,
         ICollection<LogEntry> list,
         DateTime date,
         CancellationToken cancellationToken
      )
      {
         foreach (var propertyName in entityEntry.Properties)
         {
            var context = entityEntry.Entity.GetType().Name;
            var property = propertyName.Metadata.Name;

            var previousValue = entityEntry.OriginalValues[propertyName.Metadata.Name]?.ToString();

            if (previousValue is null)
            {
               return;
            }

            list.Add(LogEntry.Create(objectId,
                                     entityEntry.Entity.GetType().Name,
                                     propertyName.Metadata.Name,
                                     previousValue,
                                     null,
                                     date,
                                     userId,
                                     LogType.Deleted,
                                     await GetRevision(context, property, cancellationToken)));
         }
      }

      private async Task<uint> GetRevision(
         string context,
         string property,
         CancellationToken cancellationToken)
      {
         var entity = _ctx.Set<LogEntry>();
         var logEntry = await entity.AsQueryable()
                                    .Where(l => l.Context == context)
                                    .Where(l => l.Property == property)
                                    .OrderByDescending(l => l.Revision)
                                    .FirstOrDefaultAsync(cancellationToken);

         return logEntry?.Revision + 1 ?? 1;
      }
}