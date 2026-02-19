using System.Net.Http;

public static class HttpRetryHelper
{
    public static bool TryGetStatus(Exception ex, out int status)
    {
        status = 0;

        if (ex is HttpRequestException httpEx)
        {
            if (httpEx.StatusCode.HasValue)
            {
                status = (int)httpEx.StatusCode.Value;
                return true;
            }

            if (httpEx.Data.Contains("StatusCode") &&
                httpEx.Data["StatusCode"] is int s)
            {
                status = s;
                return true;
            }
        }

        return false;
    }
}
