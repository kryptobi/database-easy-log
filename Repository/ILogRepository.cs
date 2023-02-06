using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DbLogger.Repository;

public interface ILogRepository
{
    Task SaveChangesWithLogAsync(Guid? userId,
                                 CancellationToken cancellationToken = default);
    Task SaveChangesWithLogAsync(Guid? userId, 
                                 IReadOnlyList<string> ignoreProperties,
                                 CancellationToken cancellationToken = default);
}