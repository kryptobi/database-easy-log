using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DbLogger.Tests;

public class CachePerContextModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(Microsoft.EntityFrameworkCore.DbContext context)
    {
        return Create(context, false);
    }

    public object Create(Microsoft.EntityFrameworkCore.DbContext context, bool designTime)
    {
        return (context, designTime);
    }
}