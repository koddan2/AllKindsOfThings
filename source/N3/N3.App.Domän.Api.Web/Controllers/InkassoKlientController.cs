using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace N3.App.Domän.Api.Web.Controllers
{
    [ApiController]
    [ApiVersion("v1")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("v1/[controller]")]
    public class InkassoKlientController : ControllerBase { }
}
