using MemobirdApp.Config;

namespace MemobirdApp.Client;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AppConfig _appConfig;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri("http://open.memobird.cn/home/");
        _appConfig = configuration.Get<AppConfig>();
    }

    public async Task<bool> PrintPaper(string content)
    {
        var req = new PrintPaperRequest(_appConfig.Ak)
        {
            MemobirdId = _appConfig.DeviceId,
            PrintContent = content,
            UserId = _appConfig.UserId
        };
        var responseMessage = await _httpClient.PostAsync("printpaper", JsonContent.Create(req));
        var resp = await responseMessage.Content.ReadFromJsonAsync<BaseResponse>();
        if (resp != null)
        {
            _logger.LogInformation("print paper response: {@resp}", resp);
            return resp.showapiResCode == 1;
        }

        return false;
    }
}