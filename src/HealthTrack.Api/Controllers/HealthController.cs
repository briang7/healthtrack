namespace HealthTrack.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/health")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Basic health check endpoint returning the API status and timestamp.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "HealthTrack API",
            Version = "1.0.0"
        });
    }
}
