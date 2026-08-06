using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Common;
using DirectoryService.Contracts.Locations;
using Microsoft.Extensions.Logging;
using Npgsql;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Queries.GetLocations;

public sealed class GetLocationsHandler(
    IReadDbConnectionFactory connectionFactory,
    ILogger<GetLocationsHandler> logger)
    : IQueryHandler<PagedResult<LocationSummaryDto>, GetLocationsQuery>
{
    public async Task<Result<PagedResult<LocationSummaryDto>, Errors>> Handle(
        GetLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var hasSearch = !string.IsNullOrWhiteSpace(query.Search);

            var hasMinDepartmentCount = query.MinDepartmentCount.HasValue;

            var orderBy = GetOrderBy(
                query.SortBy,
                query.SortDirection);

            var sql = BuildSql(
                orderBy,
                hasSearch,
                hasMinDepartmentCount);

            var offset = checked((query.Page - 1) * query.PageSize);

            var parameters = CreateParameters(
                query,
                hasSearch,
                hasMinDepartmentCount,
                offset);

            await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

            var command = new CommandDefinition(
                commandText: sql,
                parameters: parameters,
                cancellationToken: cancellationToken);

            var rows = (await connection.QueryAsync<LocationPageRow>(command)).ToArray();

            var totalCount = rows.FirstOrDefault()?.TotalCount ?? 0;

            var locations = rows
                .Where(row => row.Id.HasValue)
                .Select(MapLocation)
                .ToArray();

            return new PagedResult<LocationSummaryDto>(
                Items: locations,
                TotalCount: totalCount,
                Page: query.Page,
                PageSize: query.PageSize);
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Получение списка локаций было отменено");

            return CommonErrors
                .OperationCancelled(
                    "get.locations.was.cancelled")
                .ToErrors();
        }
        catch (NpgsqlException exception)
        {
            logger.LogError(
                exception,
                "Ошибка базы данных при получении списка локаций");

            return CommonErrors
                .Db(
                    "get.locations.from.db.exception",
                    "Не удалось получить список локаций")
                .ToErrors();
        }
    }

    private static DynamicParameters CreateParameters(
        GetLocationsQuery query,
        bool hasSearch,
        bool hasMinDepartmentCount,
        int offset)
    {
        var parameters = new DynamicParameters();

        parameters.Add(
            "PageSize",
            query.PageSize,
            DbType.Int32);

        parameters.Add(
            "Offset",
            offset,
            DbType.Int32);

        if (hasSearch)
        {
            parameters.Add(
                "SearchPattern",
                CreateSearchPattern(query.Search!),
                DbType.String);
        }

        if (hasMinDepartmentCount)
        {
            parameters.Add(
                "MinDepartmentCount",
                query.MinDepartmentCount!.Value,
                DbType.Int32);
        }

        return parameters;
    }

    private static string BuildSql(
        string orderBy,
        bool hasSearch,
        bool hasMinDepartmentCount)
    {
        var searchFilter = hasSearch
            ? """
              WHERE LOWER(l.name)
                  LIKE LOWER(@SearchPattern) ESCAPE '\'
              """
            : string.Empty;

        var departmentCountFilter = hasMinDepartmentCount
            ? """
              HAVING COUNT(DISTINCT d.id) >= @MinDepartmentCount
              """
            : string.Empty;

        return $"""
                WITH filtered_locations AS
                (
                    SELECT
                        l.id,
                        l.name,
                        CONCAT_WS(
                            ', ',
                            NULLIF(
                                l.address ->> 'PostalCode',
                                ''
                            ),
                            NULLIF(
                                l.address ->> 'Region',
                                ''
                            ),
                            NULLIF(
                                l.address ->> 'City',
                                ''
                            ),
                            NULLIF(
                                l.address ->> 'Street',
                                ''
                            ),
                            CASE
                                WHEN NULLIF(
                                    l.address ->> 'House',
                                    ''
                                ) IS NULL
                                    THEN NULL
                                ELSE
                                    'д. ' || (
                                        l.address ->> 'House'
                                    )
                            END,
                            CASE
                                WHEN NULLIF(
                                    l.address ->> 'Apartment',
                                    ''
                                ) IS NULL
                                    THEN NULL
                                ELSE
                                    'кв. ' || (
                                        l.address ->> 'Apartment'
                                    )
                            END
                        ) AS address,
                        l.created_at,
                        COUNT(
                            DISTINCT d.id
                        )::integer AS department_count
                    FROM locations AS l
                    LEFT JOIN departments_locations AS dl
                        ON dl.location_id = l.id
                    LEFT JOIN departments AS d
                        ON d.id = dl.department_id
                        AND d.is_active = TRUE
                    {searchFilter}
                    GROUP BY
                        l.id,
                        l.name,
                        l.address,
                        l.created_at
                    {departmentCountFilter}
                ),
                total AS
                (
                    SELECT
                        COUNT(*)::integer AS total_count
                    FROM filtered_locations
                ),
                paged_locations AS
                (
                    SELECT
                        fl.id,
                        fl.name,
                        fl.address,
                        fl.created_at,
                        fl.department_count,
                        ROW_NUMBER() OVER (
                            ORDER BY {orderBy}
                        ) AS row_number
                    FROM filtered_locations AS fl
                    ORDER BY
                        {orderBy}
                    LIMIT @PageSize
                    OFFSET @Offset
                )
                SELECT
                    t.total_count AS "TotalCount",
                    p.id AS "Id",
                    p.name AS "Name",
                    p.address AS "Address",
                    p.created_at AS "CreatedAt",
                    p.department_count AS "DepartmentCount"
                FROM total AS t
                LEFT JOIN paged_locations AS p
                    ON TRUE
                ORDER BY
                    p.row_number;
                """;
    }

    private static string GetOrderBy(
        string? sortBy,
        string? sortDirection)
    {
        var column = sortBy?
            .Trim()
            .ToLowerInvariant() switch
        {
            "name" =>
                "fl.name",

            "createdat" =>
                "fl.created_at",

            "departmentcount" =>
                "fl.department_count",

            _ => throw new InvalidOperationException(
                $"Неподдерживаемое поле сортировки: '{sortBy}'")
        };

        var direction = sortDirection?
            .Trim()
            .ToLowerInvariant() switch
        {
            "asc" =>
                "ASC",

            "desc" =>
                "DESC",

            _ => throw new InvalidOperationException(
                $"Неподдерживаемое направление сортировки: " +
                $"'{sortDirection}'")
        };

        return $"{column} {direction}, fl.id ASC";
    }

    private static string CreateSearchPattern(string search)
    {
        var escapedSearch = EscapeLikePattern(search.Trim());

        return $"%{escapedSearch}%";
    }

    private static string EscapeLikePattern(string value)
    {
        return value
            .Replace(
                @"\",
                @"\\",
                StringComparison.Ordinal)
            .Replace(
                "%",
                @"\%",
                StringComparison.Ordinal)
            .Replace(
                "_",
                @"\_",
                StringComparison.Ordinal);
    }

    private static LocationSummaryDto MapLocation(LocationPageRow row)
    {
        if (row.Id is null ||
            row.Name is null ||
            row.CreatedAt is null)
        {
            throw new InvalidOperationException(
                "Запрос списка локаций вернул неполные данные");
        }

        return new LocationSummaryDto(
            row.Id.Value,
            row.Name,
            row.Address ?? string.Empty,
            row.CreatedAt.Value,
            row.DepartmentCount ?? 0);
    }

    private sealed class LocationPageRow
    {
        public int TotalCount { get; set; }

        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int? DepartmentCount { get; set; }
    }
}