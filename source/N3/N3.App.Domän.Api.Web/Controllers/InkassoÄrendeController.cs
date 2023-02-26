using Microsoft.AspNetCore.Mvc;
using N3.CqrsEs.Ramverk;
using N3.CqrsEs.SkrivModell.Kommando;

namespace N3.App.Domän.Api.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InkassoÄrendeController : ControllerBase
    {
        private readonly ILogger<InkassoÄrendeController> _logger;

        public InkassoÄrendeController(
            ILogger<InkassoÄrendeController> logger,
            IKommandoHanterare<SkapaInkassoÄrendeKommando> skapaInkassoÄrendeKommandoHanterare
        )
        {
            _logger = logger;
            SkapaInkassoÄrendeKommandoHanterare = skapaInkassoÄrendeKommandoHanterare;
        }

        public IKommandoHanterare<SkapaInkassoÄrendeKommando> SkapaInkassoÄrendeKommandoHanterare { get; }

        [HttpPost]
        [Route("SkapaInassoÄrende")]
        public async Task<IActionResult> SkapaInassoÄrende(
            [FromBody] SkapaInkassoÄrendeKommando skapaInkassoÄrendeKommando
        )
        {
            using var logScope = _logger.BeginScope(skapaInkassoÄrendeKommando);
            _logger.LogTrace("Hanterar kommando {kommando}", skapaInkassoÄrendeKommando);
            await SkapaInkassoÄrendeKommandoHanterare.Hantera(skapaInkassoÄrendeKommando);
            _logger.LogTrace("Kommando {kommando} hanterat utan fel", skapaInkassoÄrendeKommando);
            return Ok(new object());
        }
    }
}
