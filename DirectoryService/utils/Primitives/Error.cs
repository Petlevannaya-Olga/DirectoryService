using System.Text.Json.Serialization;

namespace Primitives;

[method: JsonConstructor]
public record Error(string Code, string Message, ErrorType Type, string? InvalidField = null)
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public string Code { get; } = Code;

    /// <summary>
    /// Текст ошибки
    /// </summary>
    public string Message { get; } = Message;

    /// <summary>
    /// Тип ошибки
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; } = Type;

    /// <summary>
    /// Поле, в котором произошла ошибка
    /// </summary>
    public string? InvalidField { get; } = InvalidField;

    public Failure ToFailure() => this;
}

public enum ErrorType
{
    /// <summary>
    /// Отсутствие ошибки
    /// </summary>
    NONE,

    /// <summary>
    /// Ошибка валидации
    /// </summary>
    VALIDATION,

    /// <summary>
    /// Ничего не найдено
    /// </summary>
    NOT_FOUND,

    /// <summary>
    /// Серверная ошибка
    /// </summary>
    FAILURE,

    /// <summary>
    /// Конфликт
    /// </summary>
    CONFLICT,
}