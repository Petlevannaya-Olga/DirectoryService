using System.Globalization;
using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Common;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public sealed class GetDepartmentsHandler(
    IReadDbContext readDbContext,
    ILogger<GetDepartmentsHandler> logger)
    : IQueryHandler<PagedResult<DepartmentSummaryDto>, GetDepartmentsQuery>
{
    public async Task<Result<PagedResult<DepartmentSummaryDto>, Errors>> Handle(
        GetDepartmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filteredQuery = ApplySearch(
                readDbContext.DepartmentsRead,
                query.Search);

            var totalCount = await filteredQuery.CountAsync(
                cancellationToken);

            var orderedQuery = ApplySorting(
                filteredQuery,
                query.SortBy!,
                query.SortDirection!);

            var offset = (query.Page - 1) * query.PageSize;

            var departments = await orderedQuery
                .Skip(offset)
                .Take(query.PageSize)
                .Select(department => new DepartmentSummaryDto(
                    department.Id.Value,
                    department.Name.Value,
                    department.Path.Value,
                    department.CreatedAt))
                .ToListAsync(cancellationToken);

            return new PagedResult<DepartmentSummaryDto>(
                Items: departments,
                TotalCount: totalCount,
                Page: query.Page,
                PageSize: query.PageSize);
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Получение списка подразделений было отменено");

            return CommonErrors
                .OperationCancelled(
                    "get.departments.was.cancelled")
                .ToErrors();
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка получения списка подразделений");

            return CommonErrors
                .Db(
                    "get.departments.from.db.exception",
                    "Не удалось получить список подразделений")
                .ToErrors();
        }
    }

    private static IQueryable<Department> ApplySearch(
        IQueryable<Department> departments,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return departments;
        }

        var normalizedSearch = search
            .Trim()
            .ToLowerInvariant();

        var escapedSearch = EscapeLikePattern(normalizedSearch);

        var searchPattern = $"%{escapedSearch}%";

        return departments.Where(
            department =>
                EF.Functions.Like(
                    department.Name.Value.ToLower(),
                    searchPattern,
                    @"\"));
    }

    private static IOrderedQueryable<Department> ApplySorting(
        IQueryable<Department> departments,
        string sortBy,
        string sortDirection)
    {
        var normalizedSortBy = sortBy
            .Trim()
            .ToLowerInvariant();

        var normalizedSortDirection = sortDirection
            .Trim()
            .ToLowerInvariant();

        return (normalizedSortBy, normalizedSortDirection) switch
        {
            ("name", "asc") => departments
                .OrderBy(department => department.Name.Value)
                .ThenBy(department => department.Id.Value),

            ("name", "desc") => departments
                .OrderByDescending(department => department.Name.Value)
                .ThenBy(department => department.Id.Value),

            ("createdat", "asc") => departments
                .OrderBy(department => department.CreatedAt)
                .ThenBy(department => department.Id.Value),

            ("createdat", "desc") => departments
                .OrderByDescending(department => department.CreatedAt)
                .ThenBy(department => department.Id.Value),

            _ => throw new InvalidOperationException(
                $"Неподдерживаемая сортировка: " +
                $"{sortBy} {sortDirection}")
        };
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
}