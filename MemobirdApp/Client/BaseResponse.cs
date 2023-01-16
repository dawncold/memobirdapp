using System.Text.Json.Serialization;

namespace MemobirdApp.Client;

internal class BaseResponse
{
    [JsonPropertyName("showapi_res_code")]
    public int showapiResCode { get; set; }
    [JsonPropertyName("showapi_res_error")]
    public string showapiResError { get; set; }
}