using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DemoTallerApi.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class LimitationsController : ControllerBase
    {
        private readonly IpRateLimitOptions _rateLimitOptions;

        public LimitationsController(IOptions<IpRateLimitOptions> rateLimitOptions)
        {
            _rateLimitOptions = rateLimitOptions.Value;
        }

        /// <summary>
        /// Obtiene la configuración de rate limiting actual
        /// </summary>
        [HttpGet]
        public ActionResult<object> GetLimitations()
        {
            var limitations = new
            {
                RateLimiting = new
                {
                    Enabled = true,
                    Rules = _rateLimitOptions.GeneralRules.Select(r => new
                    {
                        Endpoint = r.Endpoint,
                        Period = r.Period,
                        Limit = r.Limit
                    }).ToList(),
                    HttpStatusCode = _rateLimitOptions.HttpStatusCode
                },
                ApiVersion = "1.0",
                MaxPageSize = 100,
                DefaultPageSize = 10
            };

            return Ok(limitations);
        }

        /// <summary>
        /// Obtiene el estado actual de uso del usuario (requiere autenticación)
        /// </summary>
        [HttpGet("status")]
        [Authorize]
        public ActionResult<object> GetStatus()
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            
            var status = new
            {
                ClientIp = clientIp,
                Authenticated = User.Identity?.IsAuthenticated ?? false,
                UserEmail = User.Identity?.Name ?? "Anonymous",
                Timestamp = DateTime.UtcNow,
                Message = "Rate limiting activo. Consulta /api/v1/limitations para ver los límites configurados."
            };

            return Ok(status);
        }
    }
}
