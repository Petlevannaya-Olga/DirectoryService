using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Queries.GetTopLocations;

public sealed class GetTopLocationsHandler(
    IReadDbConnectionFactory connectionFactory,
    ILogger<GetTopLocationsHandler> logger)
    : IQueryHandler<TopLocationDto[], GetTopLocationsQuery>
{
    // language=PostgreSQL
    private const string SQL =
        """
        SELECT
            l.id AS "Id",
            l.name AS "Name",
            CONCAT_WS(
                ', ',
                NULLIF(l.address ->> 'PostalCode', ''),
                NULLIF(l.address ->> 'Region', ''),
                NULLIF(l.address ->> 'City', ''),
                NULLIF(l.address ->> 'Street', ''),
                CASE
                    WHEN NULLIF(l.address ->> 'House', '') IS NULL
                        THEN NULL
                    ELSE 'д. ' || (l.address ->> 'House')
                END,
                CASE
                    WHEN NULLIF(l.address ->> 'Apartment', '') IS NULL
                        THEN NULL
                    ELSE 'кв. ' || (l.address ->> 'Apartment')
                END
            ) AS "Address",
            COUNT(DISTINCT d.id)::integer AS "DepartmentCount"
        FROM locations AS l
        LEFT JOIN departments_locations AS dl
            ON dl.location_id = l.id
        LEFT JOIN departments AS d
            ON d.id = dl.department_id
            AND d.is_active = TRUE
        WHERE l.is_active = TRUE
        GROUP BY
            l.id,
            l.name,
            l.address
        ORDER BY
            "DepartmentCount" DESC,
            l.name ASC,
            l.id ASC
        LIMIT 5;
        """;

    public async Task<Result<TopLocationDto[], Errors>> Handle(
        GetTopLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection =
                await connectionFactory.OpenConnectionAsync(
                    cancellationToken);

            var command = new CommandDefinition(
                SQL,
                cancellationToken: cancellationToken);

            var locations =
                await connection.QueryAsync<TopLocationDto>(
                    command);

            return locations.ToArray();
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Получение топа локаций было отменено");

            return CommonErrors
                .OperationCancelled(
                    "get.top.locations.was.canceled")
                .ToErrors();
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка получения топа локаций");

            return CommonErrors
                .Db(
                    "get.top.locations.from.db.exception",
                    "Не удалось получить топ локаций")
                .ToErrors();
        }
    }
}