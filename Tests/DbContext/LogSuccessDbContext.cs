using System;
using DbLogger.Domain;
using DbLogger.Tests.Misc;
using Microsoft.EntityFrameworkCore;

namespace DbLogger.Tests.DbContext;

public class LogSuccessDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    protected LogSuccessDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public LogSuccessDbContext(DbContextOptions<LogSuccessDbContext> options)
        : base(options)
    {
    }

    public DbSet<Entity> Entities { get; set; }
    public DbSet<LogEntry> Logs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<LogEntry>()
            .Property(e => e.LogTypeBy)
            .HasConversion(
                v => v.ToString(),
                v => (LogTypeBy)Enum.Parse(typeof(LogTypeBy), v));
        
        modelBuilder
            .Entity<LogEntry>()
            .Property(e => e.LogType)
            .HasConversion(
                v => v.ToString(),
                v => (LogType)Enum.Parse(typeof(LogType), v));
    }
}