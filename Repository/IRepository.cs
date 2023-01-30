using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbLogger.Repository;

public interface IRepository
{
    Task SaveChangesWithLogAsync(Guid? userId, CancellationToken cancellationToken);
}