using System.Text;
using MemobirdApp.Client;
using MemobirdApp.Model;
using MemobirdApp.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MemobirdApp.Controllers;

[ApiController]
public class PrinterController: ControllerBase
{
    private readonly ApiClient _apiClient;
    private readonly IMemoryCache _memoryCache;

    public PrinterController(ApiClient apiClient, IMemoryCache memoryCache)
    {
        _apiClient = apiClient;
        _memoryCache = memoryCache;
    }

    [HttpPost("print")]
    public async Task<IActionResult> ProcessEvent(PrintRequest request)
    {
        var succeed = await _apiClient.PrintPaper(request.Content);
        if (succeed) return Accepted();
        return BadRequest("failed to print paper");
    }

    [HttpPost("start-work")]
    public async Task<IActionResult> RecordStartWork()
    {
        var sb = new StringBuilder();
        var lastWorkItem = _memoryCache.Get<WorkItem>("keys:work");
        if (lastWorkItem == null)
        {
            // start day
            sb.AppendLine("Have a nice day");
            sb.AppendLine($"----- {DateTime.Now.ToShortDateString()} -----");
        }
        else
        {
            if (lastWorkItem.WorkItemType == WorkItemType.End)
            {
                // normal case
            }

            if (lastWorkItem.WorkItemType == WorkItemType.Start)
            {
                // forget end last work item? help to end
                var elapsed = DateTime.Now - lastWorkItem.RecordedAt;
                sb.AppendLine($"[{DateTime.Now.ToShortTimeString()}] End working");
                sb.AppendLine($"----- elapsed: {elapsed.Hours}h{elapsed.Minutes}m -----");
                if (DateTime.Now.Date > lastWorkItem.RecordedAt.Date)
                {
                    // end day
                    sb.AppendLine($"----- {lastWorkItem.RecordedAt.ToShortDateString()} -----");
                    sb.AppendLine("Have a nice sleep");
                    // start day
                    sb.AppendLine("Have a nice day"); 
                    sb.AppendLine($"----- {DateTime.Now.ToShortDateString()} -----");
                }
            }
        }

        sb.AppendLine($"[{DateTime.Now.ToShortTimeString()}] Start working");
        _memoryCache.Set("keys:work", new WorkItem
        {
            RecordedAt = DateTime.Now,
            WorkItemType = WorkItemType.Start
        });
        var succeed = await _apiClient.PrintPaper(sb.ToString());
        if (succeed) return Accepted();
        return BadRequest("something wrong");
    }

    [HttpPost("end-work")]
    public async Task<IActionResult> RecordEndWork()
    {
        var sb = new StringBuilder();
        var lastWorkItem = _memoryCache.Get<WorkItem>("keys:work");
        if (lastWorkItem == null)
        {
            // should have a start record, if not, do nothing
            return Ok();
        }
        else
        {
            if (lastWorkItem.WorkItemType == WorkItemType.End)
            {
                //do nothing
                return Ok();
            }

            if (lastWorkItem.WorkItemType == WorkItemType.Start)
            {
                var elapsed = DateTime.Now - lastWorkItem.RecordedAt;
                sb.AppendLine($"[{DateTime.Now.ToShortTimeString()}] End working");
                sb.Append($"----- elapsed: {elapsed.Hours}h{elapsed.Minutes}m -----");
                _memoryCache.Set("keys:work", new WorkItem
                {
                    RecordedAt = DateTime.Now,
                    WorkItemType = WorkItemType.End
                });
                if (DateTime.Now.Date > lastWorkItem.RecordedAt.Date)
                {
                    // end day
                    sb.AppendLine($"----- {lastWorkItem.RecordedAt.ToShortDateString()} -----");
                    sb.AppendLine("Have a nice sleep");
                }
            }
        }

        var succeed = await _apiClient.PrintPaper(sb.ToString());
        if (succeed) return Accepted();
        return BadRequest("something wrong");
    }
}