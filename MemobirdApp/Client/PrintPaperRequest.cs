namespace MemobirdApp.Client;

internal class PrintPaperRequest: BaseRequest
{
    public string PrintContent { get; set; }
    public string MemobirdId { get; set; }
    public string UserId { get; set; }

    public PrintPaperRequest(string ak) : base(ak)
    {
    }
}