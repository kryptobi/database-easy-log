using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbLogger;

public interface IRepository
{
    Task SaveChangesAsync(Guid userId, CancellationToken cancellationToken);
}