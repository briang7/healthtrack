namespace HealthTrack.Application.Common.Exceptions;

public sealed class ForbiddenException()
    : Exception("You do not have permission to perform this action.")
{
    public ForbiddenException(string message) : this()
    {
    }
}
