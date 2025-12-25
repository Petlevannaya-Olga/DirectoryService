namespace Primitives;

public record Envelope(object? Result, Errors? Errors)
{
    public bool IsError => Errors != null || (Errors != null && Errors.Any());

    public DateTime TimeGenerated { get; } = DateTime.UtcNow;

    public static Envelope Ok(object? result = null)
        => new(result, null);

    public static Envelope Error(Errors errors)
        => new(null, errors);
}

public sealed record Envelope<T>
{
    public T? Result { get; }

    public Errors? Errors { get; }

    public DateTime TimeGenerated { get; } = DateTime.UtcNow;

    public bool IsError => Errors != null || (Errors != null && Errors.Any());

    public Envelope(T? result, Errors? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }

    public static Envelope<T> Ok(T? result = default)
        => new(result, null);

    public static Envelope<T> Error(Errors errors)
        => new(default, errors);
}