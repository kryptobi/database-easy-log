using DbLogger.Repository;
using DbLogger.Tests.DbContext;

namespace DbLogger.Tests.Repositories;

public class LogSuccessRepository : RepositoryBase
{
    private readonly LogSuccessDbContext _ctx;
    
    public LogSuccessRepository(LogSuccessDbContext ctx) : base(ctx)
    {
        _ctx = ctx;
    }
    
    public LogSuccessDbContext Context()
    {
        return _ctx;
    }
}