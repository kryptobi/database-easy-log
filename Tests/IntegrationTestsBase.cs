using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DbLogger.Tests;

public class IntegrationTestsBase<T> : IDisposable
where T : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly DbConnection _connection;

    private T _testDbContext;
    protected T TestDbContext => _testDbContext ??= CreateContext();
    private readonly DbContextOptionsBuilder<T> _optionBuilder;

    private T CreateContext()
    {
        var ctx = (T)Activator.CreateInstance(typeof(T), _optionBuilder.Options)!;
        ctx.Database.OpenConnection();
        
        return ctx;
    }

#nullable disable
    protected IntegrationTestsBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:;Foreign Keys=False");

        _optionBuilder = new DbContextOptionsBuilder<T>()
                         .UseSqlite(_connection)
                         .EnableSensitiveDataLogging()
                         .ReplaceService<IModelCacheKeyFactory, CachePerContextModelCacheKeyFactory>();
    }
#nullable enable
    
    public void Dispose()
    {
        _testDbContext?.Dispose();
    }
}