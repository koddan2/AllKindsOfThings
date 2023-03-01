using Microsoft.AspNetCore.Mvc;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Anhopning;
using N3.CqrsEs.SkrivModell.Kommando;
using N3.Modell;

namespace N3.App.Domän.Api.Web.Controllers
{
    public record ImporteraInkassoÄrendeModell(
        UnikIdentifierare AktivitetsIdentifierare,
        ÄrendeImportModell ÄrendeImportModell
    ) : IAktivitet
    {
        public const string Kategori = "ImporteraInkassoÄrende";
        public AktivitetsKategori AktivitetsKategori => new(Kategori);
    }

    [ApiController]
    [Route("[controller]")]
    public class InkassoÄrendeController : ControllerBase
    {
        private readonly ILogger<InkassoÄrendeController> _logger;

        private IAktivitetsBuss AktivitetsBuss { get; }

        public InkassoÄrendeController(
            ILogger<InkassoÄrendeController> logger,
            IAktivitetsBuss aktivitetsBuss
        )
        {
            _logger = logger;
            AktivitetsBuss = aktivitetsBuss;
        }

        [HttpGet]
        [Route("Import/Status/{aktivitetsIdentifierare}")]
        public async Task<IActionResult> HämtaStatusFörImportAvInkassoÄrende(
            [FromRoute] string aktivitetsIdentifierare
        )
        {
            using var logScope = _logger.BeginScope(aktivitetsIdentifierare);
            var status = await AktivitetsBuss.HämtaStatus(aktivitetsIdentifierare);
            return Ok(status);
        }

        [HttpPost]
        [Route("Import")]
        public async Task<IActionResult> ImporteraInkassoÄrende(
            [FromBody] ImporteraInkassoÄrendeModell modell
        )
        {
            using var logScope = _logger.BeginScope(modell.AktivitetsIdentifierare);
            _logger.LogTrace("Lägger aktivitet för processering på aktivitetsbussen");
            await AktivitetsBuss.Kölägg(modell);
            return Ok(modell.AktivitetsIdentifierare);
        }
    }
}
