using Microsoft.AspNetCore.Mvc;
using N2.Domain;
using N2.Domain.DebtCollectionCase;
using N2.Domain.DebtCollectionCase.Commands;
using N2.EventSourcing;

namespace N2.Api.Http.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DebtCollectionCaseController : ControllerBase
	{
		private readonly ILogger<DebtCollectionCaseController> _logger;
		private readonly CaseAggregateCommandHandler _caseAggregateCommandHandler;

		public DebtCollectionCaseController(ILogger<DebtCollectionCaseController> logger, CaseAggregateCommandHandler caseAggregateCommandHandler)
		{
			_logger = logger;
			_caseAggregateCommandHandler = caseAggregateCommandHandler;
		}

		public record DebtCollectionCaseViewModel(string Identity, string PaymentReference);
		[HttpGet("{identity}")]
		public async Task<DebtCollectionCaseViewModel> GetSingle(string identity)
		{
			var aggregate = new CaseAggregate(identity);
			_logger.LogInformation("Hydrating");
			await _caseAggregateCommandHandler.Hydrate(aggregate);
			return new DebtCollectionCaseViewModel(identity, aggregate.Root!.PaymentReference);
		}

		public record CreateCaseViewModel(string Identity, string ClientIdentity);
		[HttpPost("Sync/{identity}")]
		public async Task CreateSync(CreateCaseViewModel vm)
		{
			var aggregate = new CaseAggregate(vm.Identity);
			var command = new CreateNewCaseCommand
			{
				ClientIdentity = vm.ClientIdentity,
			};
			_logger.LogInformation("Handling command");
			await _caseAggregateCommandHandler.Handle(aggregate, command);
		}
	}
}