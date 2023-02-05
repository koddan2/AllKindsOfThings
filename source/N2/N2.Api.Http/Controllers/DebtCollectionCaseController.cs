using Microsoft.AspNetCore.Mvc;
using N2.Api.Core;
using System.Net;

namespace N2.Api.Http.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DebtCollectionCaseController : ControllerBase
	{
		private readonly ILogger<DebtCollectionCaseController> _logger;
		private readonly N2Facade _n2Facade;

		public DebtCollectionCaseController(ILogger<DebtCollectionCaseController> logger, N2Facade n2Facade)
		{
			_logger = logger;
			_n2Facade = n2Facade;
		}

		[HttpGet("{identity}")]
		[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.NotFound)]
		public async Task<DcCaseViewModel> GetSingle(string identity)
		{
			_logger.LogInformation("Getting single");
			var a = await _n2Facade.GetSingleDcCase(identity);
			return new DcCaseViewModel(identity, a.Root!.PaymentReference);
		}

		[HttpPost("Sync/{identity}")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Conflict)]
		[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
		[ProducesErrorResponseType(typeof(ProblemDetails))]
		public async Task CreateSync(CreateDcCaseViewModel vm)
		{
			_logger.LogInformation("Handling command");
			await _n2Facade.CreateDcCase(vm);
		}
	}
}