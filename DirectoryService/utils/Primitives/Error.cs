namespace Primitives;

public sealed class Error(string code, string message)
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    /// Текст ошибки
    /// </summary>
    public string Message { get; } = message;
}