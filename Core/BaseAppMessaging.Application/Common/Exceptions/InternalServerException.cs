namespace BaseAppMessaging.Application.Common.Exceptions;

using System.Net;

using BaseAppMessaging.Domain.Primitives;


public class InternalServerException : DomainException
{
    public InternalServerException(string message, List<Error>? errors = default)
        : base(message, errors, HttpStatusCode.InternalServerError)
    {
    }
}
