namespace MemobirdApp.Request;

public class SlackEventRequest
{
    public string Token { get; set; }
    public string Challenge { get; set; }
    public string Type { get; set; }
}