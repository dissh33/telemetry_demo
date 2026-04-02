using Microsoft.AspNetCore.Mvc;
using TelemetryApi.Models.Requests;
using TelemetryApi.Services;

namespace TelemetryApi.Controllers;

[ApiController]
[Route("api/telemetry")]
[Produces("application/json")]
public class TelemetryController(ITelemetryService service) : ControllerBase
{
    /// <summary>Принять телеметрию от ТС. Типы: Coordinates, Speed, LiquidSensor, SubsystemStatus.</summary>
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitTelemetryRequest request, CancellationToken ct)
    {
        var messageId = await service.SubmitAsync(request, ct);
        return Accepted(new { messageId });
    }
}
