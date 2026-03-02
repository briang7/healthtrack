namespace HealthTrack.Application.Common.Exceptions;

public sealed class ValidationException()
    : Exception("One or more validation failures have occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

    public ValidationException(IDictionary<string, string[]> errors) : this()
    {
        Errors = errors;
    }
}
