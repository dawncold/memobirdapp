using MemobirdApp.Client;
using MemobirdApp.Request;
using Microsoft.AspNetCore.Mvc;

namespace MemobirdApp.Controllers;

[ApiController]
public class PrinterController: ControllerBase
{
    private readonly ApiClient _apiClient;

    public PrinterController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpPost("print")]
    public async Task<IActionResult> ProcessEvent(PrintRequest request)
    {
        var succeed = await _apiClient.PrintPaper(request.Content);
        if (succeed) return Accepted();
        return BadRequest("failed to print paper");
    }
}