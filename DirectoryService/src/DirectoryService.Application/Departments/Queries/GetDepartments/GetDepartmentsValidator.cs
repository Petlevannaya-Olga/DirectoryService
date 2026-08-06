using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public sealed class GetDepartmentsQueryValidator
    : AbstractValidator<GetDepartmentsQuery>
{
    private const int MAX_SEARCH_LENGTH = 100;
    private const int MAX_PAGE_SIZE = 100;

    private static readonly string[] _allowedSortFields =
    [
        "name",
        "createdAt"
    ];

    private static readonly string[] _allowedSortDirections =
    [
        "asc",
        "desc"
    ];

    public GetDepartmentsQueryValidator()
    {
        RuleFor(query => query.Search)
            .MaximumLength(MAX_SEARCH_LENGTH)
            .WithError(
                CommonErrors.Validation(
                    nameof(GetDepartmentsQuery.Search),
                    $"Поисковая строка не должна превышать {MAX_SEARCH_LENGTH} символов"));

        RuleFor(query => query.SortBy)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithError(
                CommonErrors.IsRequired(nameof(GetDepartmentsQuery.SortBy)))
            .Must(IsAllowedSortField)
            .WithError(
                CommonErrors.Validation(
                    nameof(GetDepartmentsQuery.SortBy),
                    "Допустимые значения поля сортировки: name, createdAt"));

        RuleFor(query => query.SortDirection)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired(nameof(GetDepartmentsQuery.SortDirection)))
            .Must(IsAllowedSortDirection)
            .WithError(
                CommonErrors.Validation(
                    nameof(GetDepartmentsQuery.SortDirection),
                    "Допустимые направления сортировки: asc, desc"));

        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1)
            .WithError(
                CommonErrors.Validation(
                    nameof(GetDepartmentsQuery.Page),
                    "Номер страницы должен быть больше или равен 1"));

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, MAX_PAGE_SIZE)
            .WithError(
                CommonErrors.Validation(
                    nameof(GetDepartmentsQuery.PageSize),
                    $"Размер страницы должен находиться в диапазоне от 1 до {MAX_PAGE_SIZE}"));

        RuleFor(query => query)
            .Must(HasValidOffset)
            .When(query =>
                query.Page >= 1 &&
                query.PageSize is >= 1 and <= MAX_PAGE_SIZE)
            .WithError(
                CommonErrors.Validation(
                    nameof(GetDepartmentsQuery.Page),
                    "Запрошен слишком большой номер страницы"));
    }

    private static bool IsAllowedSortField(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return false;
        }

        return _allowedSortFields.Contains(
            sortBy.Trim(),
            StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsAllowedSortDirection(string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortDirection))
        {
            return false;
        }

        return _allowedSortDirections.Contains(
            sortDirection.Trim(),
            StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasValidOffset(GetDepartmentsQuery query)
    {
        var offset = (long)(query.Page - 1) * query.PageSize;
        return offset <= int.MaxValue;
    }
}