using System;
using System.ComponentModel.DataAnnotations;
using DbLogger.Domain;

namespace DbLogger;

public class LogEntry
{
    private const int MAX_LENGTH_VALUE_TYPES = 255;
    private const int MAX_LENGHT = 10;
    [Key] public Guid Id { get; private init; }
    public Guid ContextId { get; private set; }
    public string Context { get; private set; }
    [MaxLength(MAX_LENGTH_VALUE_TYPES)] public string Property { get; private set; }
    [MaxLength(MAX_LENGTH_VALUE_TYPES)] public string? PreviousValue { get; private set; }
    [MaxLength(MAX_LENGTH_VALUE_TYPES)] public string? CurrentValue { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid? ChangedBy { get; private init; }
    [MaxLength(MAX_LENGHT)] public LogType LogType { get; set; }
    [MaxLength(MAX_LENGHT)] public LogTypeBy LogTypeBy { get; set; }
    public uint Revision { get; set; }

    private LogEntry(
        Guid contextId,
        string context,
        string property,
        string? previousValue,
        string? currentValue,
        DateTime changedAt,
        Guid? changedBy,
        LogType logType,
        LogTypeBy logTypeBy,
        uint revision
    )
    {
        Id = Guid.NewGuid();
        Context = context;
        ContextId = contextId;
        Property = property;
        PreviousValue = previousValue;
        CurrentValue = currentValue;
        ChangedAt = changedAt;
        ChangedBy = changedBy;
        LogType = logType;
        LogTypeBy = logTypeBy;
        Revision = revision;
    }

    public static LogEntry Create(
        Guid contextId,
        string context,
        string property,
        string? previousValue,
        string? currentValue,
        DateTime changedAt,
        Guid? changedByUser,
        LogType logType,
        LogTypeBy logTypeBy,
        uint revision
    )
    {
        return new LogEntry(contextId,
                            context,
                            property,
                            previousValue,
                            currentValue,
                            changedAt,
                            changedByUser,
                            logType,
                            logTypeBy,
                            revision
                           );
    }

    public LogEntry SetRevision(uint revision)
    {
        Revision = revision;

        return this;
    }
}