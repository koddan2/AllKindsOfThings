using Microsoft.AspNetCore.Mvc;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Anhopning;
using N3.CqrsEs.SkrivModell.Kommando;
using N3.Modell;

namespace N3.App.Domän.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InkassoÄrendeController : ControllerBase
    {
        private readonly ILogger<InkassoÄrendeController> _logger;

        public IAktivitetsBuss AktivitetsBuss { get; }

        public InkassoÄrendeController(
            ILogger<InkassoÄrendeController> logger,
            IAktivitetsBuss aktivitetsBuss
        )
        {
            _logger = logger;
            AktivitetsBuss = aktivitetsBuss;
        }

        public record ImporteraInkassoÄrendeModell(
            UnikIdentifierare AktivitetsIdentifierare,
            ÄrendeImportModell ÄrendeImportModell
        ) : IAktivitet;

        [HttpPost]
        [Route("SkapaInkassoÄrende")]
        public async Task<IActionResult> ImporteraInkassoÄrende(
            [FromBody] ImporteraInkassoÄrendeModell modell
        )
        {
            using var logScope = _logger.BeginScope(modell.AktivitetsIdentifierare);
            _logger.LogTrace("Lägger aktivitet för processering på aktivitetsbussen");
            await AktivitetsBuss.Registrera(modell);
            return Ok(modell.AktivitetsIdentifierare);
        }
    }
}
