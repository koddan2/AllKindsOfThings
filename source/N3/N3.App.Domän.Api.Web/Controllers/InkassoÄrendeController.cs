using Marten;
using Microsoft.AspNetCore.Mvc;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.JobbPaket;
using N3.CqrsEs.SkrivModell.Kommando;
using N3.Modell;

namespace N3.App.Domän.Api.Web.Controllers
{
    [ApiController]
    [ApiVersion("v1")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("v1/[controller]")]
    public class InkassoÄrendeController : ControllerBase
    {
        private readonly ILogger<InkassoÄrendeController> _logger;
        private readonly IDocumentStore _store;
        private readonly IJobbKö _aktivitetsBuss;

        public InkassoÄrendeController(
            ILogger<InkassoÄrendeController> logger,
            IDocumentStore store,
            IJobbKö aktivitetsBuss
        )
        {
            _logger = logger;
            _store = store;
            _aktivitetsBuss = aktivitetsBuss;
        }

        [HttpGet]
        [Route("Import/Status/Alla")]
        [ProducesResponseType(200, Type = typeof(ImporteraInkassoÄrendeModell[]))]
        public async Task<IActionResult> HämtaStatusFörImportAvInkassoÄrenden()
        {
            using var logScope = _logger.BeginScope("Import/Status/Alla");
            var data = await _aktivitetsBuss.HämtaStatus<ImporteraInkassoÄrendeModell>();
            return Ok(data);
        }

        [HttpGet]
        [Route("Import/Status/{aktivitetsIdentifierare}")]
        [ProducesResponseType(200, Type = typeof(ImporteraInkassoÄrendeModell))]
        [ProducesResponseType(404, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> HämtaStatusFörImportAvInkassoÄrende(
            [FromRoute] string aktivitetsIdentifierare
        )
        {
            using var logScope = _logger.BeginScope(aktivitetsIdentifierare);
            var data = await _aktivitetsBuss.HämtaStatus<ImporteraInkassoÄrendeModell>(
                aktivitetsIdentifierare
            );
            return Ok(data);
        }

        public record KöläggningsKvitto(string Id);

        public record KonfliktId(string Id);

        [HttpPost]
        [Route("Import/Kölägg")]
        [ProducesResponseType(200, Type = typeof(KöläggningsKvitto))]
        [ProducesResponseType(400, Type = typeof(ProblemDetails))]
        [ProducesResponseType(409, Type = typeof(KonfliktId))]
        public async Task<IActionResult> ImporteraInkassoÄrende(
            [FromBody] ImporteraInkassoÄrendeModell modell
        )
        {
            using var logScope = _logger.BeginScope(modell.Id);
            _logger.LogTrace(
                "Lägger aktivitet för processering på aktivitetsbussen (id={id})",
                modell.Id
            );
            await _aktivitetsBuss.Kölägg(modell);
            return Ok(new KöläggningsKvitto(modell.Id));
        }
    }
}
