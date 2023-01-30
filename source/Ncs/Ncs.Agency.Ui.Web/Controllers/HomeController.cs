using EventStore.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ncs.Domain.Model;
using System.Text.Json;
using SAK;

namespace Ncs.Agency.Ui.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger _logger;
		private readonly EventStoreClient _eventStoreClient;

		public HomeController(ILogger<HomeController> logger, EventStoreClient eventStoreClient)
		{
			_logger = logger;
			_eventStoreClient = eventStoreClient;
		}

		public async Task<ActionResult> Index()
		{
			var metadata = await _eventStoreClient.GetStreamMetadataAsync("debt-collection-client");
			return View();
		}

		public async Task<ActionResult> Details(string id)
		{
			var metadata = await _eventStoreClient.GetStreamMetadataAsync("debt-collection-client");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([FromForm] DebtCollectionClientCreateModel_V1 modelCreate)
		{
			var @event = new DebtCollectionClientCreatedEvent_V1(
				Guid.NewGuid().ToString("N"),
				modelCreate);

			var eventData = new EventData(
				Uuid.NewUuid(),
				nameof(DebtCollectionClientCreatedEvent_V1),
				JsonSerializer.SerializeToUtf8Bytes(@event)
			);

			var result = await _eventStoreClient.AppendToStreamAsync(
				"debt-collection-client",
				StreamState.Any,
				new[] { eventData },
				cancellationToken: HttpContext.RequestAborted);
			_logger.LogInformation("Result: {result}", result.ToJson());
			return RedirectToAction(nameof(Index), new { ClientId = @event.Id });
		}
	}
}
