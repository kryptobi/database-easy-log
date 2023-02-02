using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DbLogger.Domain;
using DbLogger.Tests.DbContext;
using DbLogger.Tests.Misc;
using DbLogger.Tests.Repositories;
using Xunit;
using FluentAssertions;

namespace DbLogger.Tests.SaveChangesWithLog;

public class Success : IntegrationTestsBase<LogSuccessDbContext>
{
    private readonly LogSuccessRepository _logSuccessRepository;
    private readonly LogSuccessDbContext _logSuccessDbContext;

    public Success()
    {
        _logSuccessDbContext = TestDbContext;
        _logSuccessRepository = new LogSuccessRepository(_logSuccessDbContext);
    }

    [Fact]
    public async Task Should_Create_Log()
    {
        var entity = new Entity("Start");
        var userId = Guid.NewGuid();

        _logSuccessRepository.Context().Add(entity);
        await _logSuccessRepository.SaveChangesWithLogAsync(userId, CancellationToken.None);

        var resultId = _logSuccessDbContext.Logs.First(l => l.Property == "Id");

        resultId.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_Create_Log_By_User()
    {
        var entity = new Entity("Start");
        var userId = Guid.NewGuid();

        _logSuccessRepository.Context().Add(entity);
        await _logSuccessRepository.SaveChangesWithLogAsync(userId, CancellationToken.None);

        var resultId = _logSuccessDbContext.Logs.First(l => l.Property == "Id");

        resultId.Should().NotBeNull();
        resultId.LogTypeBy.Should().Be(LogTypeBy.User);
    }

    [Fact]
    public async Task Should_Create_Log_By_System()
    {
        var entity = new Entity("Start");

        _logSuccessRepository.Context().Add(entity);
        await _logSuccessRepository.SaveChangesWithLogAsync(null, CancellationToken.None);

        var resultId = _logSuccessDbContext.Logs.First(l => l.Property == "Id");

        resultId.Should().NotBeNull();
        resultId.LogTypeBy.Should().Be(LogTypeBy.System);
    }
}