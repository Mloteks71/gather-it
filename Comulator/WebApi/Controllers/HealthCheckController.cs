using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthCheckController : BaseController
{
    public HealthCheckController(ILogger<HealthCheckController> logger) : base(logger)
    {
    }

    [HttpGet]
    public ActionResult Get()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
