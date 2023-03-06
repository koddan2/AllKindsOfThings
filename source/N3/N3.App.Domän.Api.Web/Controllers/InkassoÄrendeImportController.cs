using Microsoft.AspNetCore.Mvc;
using N3.App.Domän.Api.Web.Messages;
using N3.CqrsEs.Ramverk.Jobs;
using N3.CqrsEs.SkrivModell.JobbPaket;
using Rebus.Bus;

namespace N3.App.Domän.Api.Web.Controllers
{
    [ApiController]
    [ApiVersion("v1")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("v1/[controller]")]
    public class InkassoÄrendeImportController : ControllerBase
    {
        private readonly ILogger<InkassoÄrendeImportController> _logger;
        private readonly IBus _bus;
        private readonly IJobQueue _jobbKö;

        public InkassoÄrendeImportController(
            ILogger<InkassoÄrendeImportController> logger,
            IBus bus,
            IJobQueue jobbKö
        )
        {
            _logger = logger;
            _bus = bus;
            _jobbKö = jobbKö;
        }

        [HttpGet]
        [Route("Status/Alla")]
        [ProducesResponseType(200, Type = typeof(JobStatus[]))]
        public async Task<IActionResult> HämtaStatusFörImportAvInkassoÄrenden()
        {
            using var logScope = _logger.BeginScope(HämtaStatusFörImportAvInkassoÄrenden);
            var data = await _jobbKö.GetStatus<ImporteraInkassoÄrendeJobbData>();
            return Ok(data);
        }

        [HttpGet]
        [Route("Status/{aktivitetsIdentifierare}")]
        [ProducesResponseType(200, Type = typeof(JobStatus))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> HämtaStatusFörImportAvInkassoÄrende(
            [FromRoute] string aktivitetsIdentifierare
        )
        {
            using var logScope = _logger.BeginScope(HämtaStatusFörImportAvInkassoÄrende);
            var data = await _jobbKö.GetStatus<ImporteraInkassoÄrendeJobbData>(
                aktivitetsIdentifierare
            );
            return Ok(data);
        }

        public record KöläggningsKvitto(string Id);

        public record KonfliktId(string Id);

        public record ImporteraInkassoÄrendeModell(ImporteraInkassoÄrendeJobbData Jobb);

        [HttpPost]
        [Route("Kölägg")]
        [ProducesResponseType(200, Type = typeof(KöläggningsKvitto))]
        [ProducesResponseType(400, Type = typeof(ProblemDetails))]
        [ProducesResponseType(409, Type = typeof(KonfliktId))]
        public async Task<IActionResult> KöläggImportAvInkassoÄrende(
            [FromBody] ImporteraInkassoÄrendeModell modell
        )
        {
            using var logScope = _logger.BeginScope(KöläggImportAvInkassoÄrende);
            await _bus.SendLocal(new ImporteraInkassoÄrendeJobbKommando(modell.Jobb));
            return Ok(new KöläggningsKvitto(modell.Jobb.Id));
        }
    }
}
