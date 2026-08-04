namespace Primitives;

public sealed record Envelope<T>
{
    public T? Result { get; }

    public Errors? Errors { get; }

    public bool IsError => Errors is { Count: > 0 };

    public DateTime TimeGenerated { get; }

    private Envelope(
        T? result,
        Errors? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }

    public static Envelope<T> Ok(T? result = default)
    {
        return new Envelope<T>(
            result,
            errors: null);
    }

    public static Envelope<T> Error(Errors errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        return new Envelope<T>(
            result: default,
            errors);
    }
}