using Microsoft.AspNetCore.Mvc;
using N2.Api.Core;
using N2.Domain;
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
		public async Task<IEnumerable<EventReadResult>> GetLogForSingle(string identity)
		{
			_logger.LogInformation("Getting log");
			var listOfEventReadResults = await _n2Facade.DcCaseGetLogSingle(identity);
			return listOfEventReadResults;
		}

		[HttpPost("Sync/{identity}")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.Conflict)]
		[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
		[ProducesErrorResponseType(typeof(ProblemDetails))]
		public async Task CreateSync(DcCaseViewModelCreate vm)
		{
			_logger.LogInformation("Handling command");
			await _n2Facade.DcCaseCreate(vm);
		}
	}
}