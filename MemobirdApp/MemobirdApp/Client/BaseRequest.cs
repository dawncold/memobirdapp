namespace MemobirdApp.Client;

internal class BaseRequest
{
    public BaseRequest(string ak)
    {
        Ak = ak;
        Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public string Ak { get; }
    public string Timestamp { get; }
}