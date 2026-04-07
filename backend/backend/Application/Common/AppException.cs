namespace WebShortlink.Backend.Application.Common;

public sealed class AppException : Exception
{
    public AppException(string errorCode, string message, int statusCode = StatusCodes.Status400BadRequest)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    public string ErrorCode { get; }
    public int StatusCode { get; }
}
