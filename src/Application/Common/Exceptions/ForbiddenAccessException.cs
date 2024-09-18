namespace Offers.CleanArchitecture.Application.Common.Exceptions;

public class ForbiddenAccessException : UnauthorizedAccessException
{
    public ForbiddenAccessException() : base() { }
}
