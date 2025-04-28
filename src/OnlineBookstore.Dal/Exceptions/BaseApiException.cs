namespace OnlineBookstore.Dal.Exceptions;
public abstract class BaseApiException : Exception
{
    public int StatusCode { get; }
    public string ErrorCode { get; }

    protected BaseApiException(string message, int statusCode, string errorCode = "")
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}