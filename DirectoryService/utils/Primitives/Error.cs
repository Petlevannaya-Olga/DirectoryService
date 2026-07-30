using System.Text.Json.Serialization;

namespace Primitives;

public sealed record Error(
    string Code,
    string Message,
    [property: JsonConverter(typeof(JsonStringEnumConverter<ErrorType>))]
    ErrorType Type,
    string? InvalidField = null)
{
    private const string Separator = "||";

    public Errors ToErrors() => this;

    public string Serialize() =>
        string.Join(Separator, Code, Message, Type);

    public static Error Deserialize(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        string[] parts = value.Split(
            Separator,
            StringSplitOptions.None);

        if (parts.Length != 3 ||
            !Enum.TryParse(parts[2], out ErrorType errorType))
        {
            throw new FormatException(
                "Invalid serialized Error format.");
        }

        return new Error(
            parts[0],
            parts[1],
            errorType);
    }
}