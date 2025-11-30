using Microsoft.AspNetCore.Mvc;

namespace Detective.Controllers;

[ApiController]
[Route("api/v1/health")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(DateTime.Now);
    }
}