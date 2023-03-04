using Microsoft.AspNetCore.Mvc;

namespace N3.App.Domän.Api.Web.Controllers
{
    [ApiController]
    [ApiVersion("v1")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("v1/[controller]")]
    public class InkassoKlientController : ControllerBase
    {
        private readonly ILogger<InkassoKlientController> _logger;

        public InkassoKlientController(ILogger<InkassoKlientController> logger)
        {
            _logger = logger;
        }
    }
}
