using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseEasyLog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DatabaseEasyLog.Repository;

public abstract class LogRepositoryBase : ILogRepository
{
    private readonly DbContext _ctx;

    protected LogRepositoryBase(DbContext ctx)
    {
       _ctx = ctx;
    }
    
    /// <inheritdoc />
    public async Task SaveChangesWithLogAsync(Guid? userId, CancellationToken cancellationToken = default)
    {
        EnsureValid();

        var logEntries = await LogEntries(userId,
                                          _ctx.ChangeTracker.Entries().ToList(),
                                          new List<string>(),
                                          cancellationToken);

        _ctx.AddRange(logEntries);
        await _ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesWithLogAsync(Guid? userId,
                                              IReadOnlyList<string> ignoreProperties,
                                              CancellationToken cancellationToken = default)
    {
        EnsureValid();

        var logEntries = await LogEntries(
                                          userId,
                                          _ctx.ChangeTracker.Entries().ToList(),
                                          ignoreProperties,
                                          cancellationToken);

        _ctx.AddRange(logEntries);
        await _ctx.SaveChangesAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<LogEntry>> LogEntries(
        Guid? userId,
        IReadOnlyList<EntityEntry> entries,
        IReadOnlyList<string> ignoreProperties,
        CancellationToken cancellationToken = default)
    {
        if (!entries.Any())
        {
            return new Collection<LogEntry>();
        }

        var date = DateTime.UtcNow;
        var list = new Collection<LogEntry>();
        foreach (var entityEntry in entries)
        {
            if (entityEntry.CurrentValues.TryGetValue("Id", out Guid? objectId) == false)
            {
                objectId = null;
            }

            switch (entityEntry.State)
            {
                case EntityState.Deleted:
                {
                    await GetDeletedEntryProperties(userId,
                                                    objectId,
                                                    entityEntry,
                                                    list,
                                                    ignoreProperties,
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
                                                  ignoreProperties,
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
                                                     ignoreProperties,
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

        return list;
    }

    private void EnsureValid()
    {
        try
        {
            _ = _ctx.Set<LogEntry>();
        }
        catch (Exception e)
        {
            throw new Exception($"No Log Table configured. Message: {e.Message}");
        }
    }

    private async Task GetModifiedEntryProperties(
        Guid? userId,
        Guid? objectId,
        EntityEntry entityEntry,
        ICollection<LogEntry> list,
        IReadOnlyList<string> ignoreProperties,
        DateTime date,
        CancellationToken cancellationToken
    )
    {
        foreach (var propertyName in entityEntry.Properties
                                                .Where(p => p.IsModified 
                                                            && ignoreProperties.Contains(p.Metadata.Name))
                                                )
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
                                     userId == null ? LogTypeBy.System : LogTypeBy.User,
                                     await GetRevision(context, property, cancellationToken)
                                    ));
        }
    }

    private async Task GetAddedEntryProperties(
        Guid? userId,
        Guid? objectId,
        EntityEntry entityEntry,
        ICollection<LogEntry> entries,
        IReadOnlyList<string> ignoreProperties,
        DateTime date,
        CancellationToken cancellationToken
    )
    {
        foreach (var propertyName in entityEntry.Properties
                                                .Where(p => ignoreProperties.Contains(p.Metadata.Name)))
        {
            var context = entityEntry.Entity.GetType().Name;
            var property = propertyName.Metadata.Name;

            var currentValue = entityEntry.CurrentValues[propertyName.Metadata.Name]?.ToString();

            if (currentValue is null)
            {
                return;
            }

            entries.Add(LogEntry.Create(objectId,
                                        entityEntry.Entity.GetType().Name,
                                        propertyName.Metadata.Name,
                                        null,
                                        currentValue,
                                        date,
                                        userId,
                                        LogType.Added,
                                        userId == null ? LogTypeBy.System : LogTypeBy.User,
                                        await GetRevision(context, property, cancellationToken)));
        }
    }

    private async Task GetDeletedEntryProperties(
        Guid? userId,
        Guid? objectId,
        EntityEntry entityEntry,
        ICollection<LogEntry> list,
        IReadOnlyList<string> ignoreProperties,
        DateTime date,
        CancellationToken cancellationToken
    )
    {
        foreach (var propertyName in entityEntry.Properties.Where(p => ignoreProperties.Contains(p.Metadata.Name)))
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
                                     userId == null ? LogTypeBy.System : LogTypeBy.User,
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