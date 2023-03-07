using Microsoft.AspNetCore.Mvc;
using N3.App.Domän.Api.Web.ApiModels;
using N3.App.Domän.Api.Web.Messages;
using Rebus.Bus;

namespace N3.App.Domän.Api.Web.Controllers
{
    internal static class ControllerBaseExtensions
    {
        public static string GetCorrelationIdOrNew(this ControllerBase ctr)
        {
            var found = ctr.Request.Headers.TryGetValue("correlation-id", out var values);
            List<string> result = new();
            if (found)
            {
                foreach (var item in values)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        result.Add(item);
                    }
                }
            }

            if (result.Count == 0)
            {
                result.Add(UnikIdentifierare.Skapa());
            }

            return result.Last();
        }
    }

    [ApiController]
    [ApiVersion("v1")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("v1/[controller]")]
    public class InkassoKlientController : ControllerBase
    {
        private readonly ILogger<InkassoKlientController> _logger;
        private readonly IBus _bus;

        public InkassoKlientController(ILogger<InkassoKlientController> logger, IBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpPost]
        [Route("Registrera")]
        public async Task<IActionResult> RegistreraNyKlient(
            RegistreraNyInkassoKlientApiModell modell
        )
        {
            using var logScope = _logger.BeginScope(RegistreraNyKlient);
            var commandMessage = new RegistreraNyInkassoKlientKommando(
                UnikIdentifierare.Skapa(),
                modell
            )
            {
                KorrelationsIdentifierare = this.GetCorrelationIdOrNew()
            };
            await _bus.SendLocal(commandMessage);
            return Ok(new Receipt(commandMessage.Id));
        }
    }
}
