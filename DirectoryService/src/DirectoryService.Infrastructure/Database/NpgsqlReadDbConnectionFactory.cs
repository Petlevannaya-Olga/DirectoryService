using System.Data.Common;
using DirectoryService.Application.Database;
using Npgsql;

namespace DirectoryService.Infrastructure.Database;

public sealed class NpgsqlReadDbConnectionFactory(
    NpgsqlDataSource dataSource)
    : IReadDbConnectionFactory
{
    public async Task<DbConnection> OpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        return await dataSource.OpenConnectionAsync(
            cancellationToken);
    }
}