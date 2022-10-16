namespace DailyWireApi.Exceptions;

public class DailyWireApiException : Exception
{
    public DailyWireApiException(string? message) : base(message)
    {
    }
}