using EventStore.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ncs.Domain.Model;
using System.Text.Json;
using SAK;
using Ncs.EventSourcing;
using System.Text;

namespace Ncs.Agency.Ui.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger _logger;
		private readonly IEventStore _eventStore;
		private readonly IUniqueIdGenerator _uniqueIdGenerator;

		public HomeController(ILogger<HomeController> logger, IEventStore eventStore, IUniqueIdGenerator uniqueIdGenerator)
		{
			_logger = logger;
			_eventStore = eventStore;
			_uniqueIdGenerator = uniqueIdGenerator;
		}

		public async Task<ActionResult> Index()
		{
			await Task.CompletedTask;
			return View();
		}

		public async Task<ActionResult> Details([FromQuery] string clientId)
		{
			var currentClient = new DebtCollectionClientAggregate(clientId);
			var eventsAsync = _eventStore.ReadStreamFullAsync(DebtCollectionClientAggregate.AggregateName, clientId);
			//await foreach (var @event in eventsAsync)
			//{
			//	currentClient.Apply(@event.EventType, Encoding.UTF8.GetString(@event.Data.ToArray()));
			//}
			var events = await eventsAsync.ToListAsync();
			foreach (var @event in events)
			{
				currentClient.Apply(@event.EventType, Encoding.UTF8.GetString(@event.Data.ToArray()));
			}
			return View(currentClient);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([FromForm] DebtCollectionClientCreateModel_V1 modelCreate)
		{
			var @event = new DebtCollectionClientCreatedEvent_V1(
				_uniqueIdGenerator.MakeOne(),
				modelCreate.PersonalIdentificationNumber,
				modelCreate.Name);
			var result = await _eventStore.StoreEventAsync(@event, HttpContext.RequestAborted);
			_logger.LogInformation("Result: {result}", result.ToJson());
			return RedirectToAction(nameof(Details), new { clientId = @event.Id });
		}
	}
}
