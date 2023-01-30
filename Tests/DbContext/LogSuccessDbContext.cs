using DbLogger.Tests.Misc;
using Microsoft.EntityFrameworkCore;

namespace DbLogger.Tests.DbContext;

public class LogSuccessDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Entity> Entites { get; set; }
    public DbSet<LogEntry> Logs { get; set; }
}