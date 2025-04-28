using System.Net;

namespace OnlineBookstore.Dal.Exceptions;

public class NotFoundException : BaseApiException
{
    public NotFoundException(string message)
            : base(message, (int)HttpStatusCode.NotFound)
    {
    }
}
