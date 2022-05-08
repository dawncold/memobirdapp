using MemobirdApp.Response;
using Microsoft.AspNetCore.Mvc;

namespace MemobirdApp.Controllers;

[ApiController]
[Route("slack/events")]
public class SlackEventController: ControllerBase
{
    [HttpPost]
    public SlackEventResponse ProcessEvent(SlackEventResponse request)
    {
        return new SlackEventResponse
        {
            Challenge = request.Challenge
        };
    }
}