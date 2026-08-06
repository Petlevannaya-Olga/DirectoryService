using System.Data.Common;

namespace DirectoryService.Application.Database;

public interface IReadDbConnectionFactory
{
    Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}