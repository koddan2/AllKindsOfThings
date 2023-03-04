using Microsoft.AspNetCore.Mvc;
using SlimMessageBus;

namespace N3.App.Domän.Api.Web.Controllers
{
    [ApiController]
    [ApiVersion("v1")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("v1/[controller]")]
    public class InkassoKlientController : ControllerBase
    {
        private readonly ILogger<InkassoKlientController> _logger;
        private readonly IMessageBus _bus;

        public InkassoKlientController(ILogger<InkassoKlientController> logger, IMessageBus bus)
        {
            _logger = logger;
            _bus = bus;
        }
    }
}
