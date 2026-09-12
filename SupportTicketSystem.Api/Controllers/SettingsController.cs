using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.Infrastructure.Services;

namespace SupportTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("/api/settings")]
    public class SettingsController(SettingsService service) : ControllerBase
    {
        [HttpGet("")]
        public async Task<IActionResult> config()
        {
            var res = service.getSetting();

            return Ok(res);
        }
    }
}
