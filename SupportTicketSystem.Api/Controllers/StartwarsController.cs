using Microsoft.AspNetCore.Mvc;
using SupportTicketSystem.Core.Responses;
using SupportTicketSystem.Infrastructure.ExternalApi;
using System.Threading.Tasks;

namespace SupportTicketSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StartwarsController : ControllerBase
    {
        private readonly StarWarsApi _starWarsApi;

        public StartwarsController(StarWarsApi starWarsApi)
        {
            _starWarsApi = starWarsApi;
        }

        [HttpGet("call")]
        public async Task<IActionResult> Call()
        {
            string path = await _starWarsApi.CallExternalApiAsync();

            ApiResponse res = new ApiResponse(201, "Starwars api called & data is written in file");
            return Created(path, res);
        }
    }
}
