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
    private readonly TimeZoneInfo _cstTZ;
    private const string CacheKey = "keys:work";

    public PrinterController(ApiClient apiClient, IMemoryCache memoryCache)
    {
        _apiClient = apiClient;
        _memoryCache = memoryCache;
        _cstTZ = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
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
        var lastWorkItem = _memoryCache.Get<WorkItem>(CacheKey);
        if (lastWorkItem == null)
        {
            // start day
            sb.AppendLine("Have a nice day");
            sb.AppendLine($"----- {GetLocalDateString()} -----");
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
                var elapsed = DateTime.UtcNow - lastWorkItem.RecordedAt;
                sb.AppendLine($"[{GetLocalTimeString()}] End working");
                sb.AppendLine($"----- elapsed: {GetElapsedString(elapsed)} -----");
                if (DateTime.UtcNow.Date > lastWorkItem.RecordedAt.Date)
                {
                    // end day
                    sb.AppendLine($"----- {lastWorkItem.RecordedAt.ToString("MM/dd/yyyy")} -----");
                    sb.AppendLine("Have a nice sleep");
                    // start day
                    sb.AppendLine("Have a nice day"); 
                    sb.AppendLine($"----- {GetLocalDateString()} -----");
                }
            }
        }

        sb.AppendLine($"[{GetLocalTimeString()}] Start working");
        RecordWorkItem(WorkItemType.Start);
        var succeed = await _apiClient.PrintPaper(sb.ToString());
        if (succeed) return Accepted();
        return BadRequest("something wrong");
    }

    [HttpPost("end-work")]
    public async Task<IActionResult> RecordEndWork()
    {
        var sb = new StringBuilder();
        var lastWorkItem = _memoryCache.Get<WorkItem>(CacheKey);
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
                var elapsed = DateTime.UtcNow - lastWorkItem.RecordedAt;
                sb.AppendLine($"[{GetLocalTimeString()}] End working");
                sb.Append($"----- elapsed: {GetElapsedString(elapsed)} -----");
                RecordWorkItem(WorkItemType.End);
                if (DateTime.UtcNow.Date > lastWorkItem.RecordedAt.Date)
                {
                    // end day
                    sb.AppendLine($"----- {lastWorkItem.RecordedAt.ToString("MM/dd/yyyy")} -----");
                    sb.AppendLine("Have a nice sleep");
                }
            }
        }

        var succeed = await _apiClient.PrintPaper(sb.ToString());
        if (succeed) return Accepted();
        return BadRequest("something wrong");
    }

    private string GetLocalDateString()
    {
        var dt = DateTime.UtcNow;
        return TimeZoneInfo.ConvertTimeFromUtc(dt, _cstTZ).ToString("MM/dd/yyyy");
    }

    private string GetLocalTimeString()
    {
        var dt = DateTime.UtcNow;
        return TimeZoneInfo.ConvertTimeFromUtc(dt, _cstTZ).ToString("h:mm tt");
    }

    private void RecordWorkItem(WorkItemType type)
    {
        _memoryCache.Set(CacheKey, new WorkItem
        {
            RecordedAt = DateTime.UtcNow,
            WorkItemType = type
        });
    }

    private string GetElapsedString(TimeSpan span)
    {
        var sb = new StringBuilder();
        if (span.Hours > 0)
        {
            sb.Append($"{span.Hours}h");
        }

        if (span.Minutes > 0)
        {
            sb.Append($"{span.Minutes}m");
        }
        else
        {
            if (span.Seconds > 0)
            {
                if (span.Hours > 0)
                {
                    sb.Append($"{span.Minutes}m{span.Seconds}s");
                }
                else
                {
                    sb.Append($"{span.Seconds}s");
                }
            }
        }
        return sb.ToString();
    }
}