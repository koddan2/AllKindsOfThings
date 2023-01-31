using EventStore.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ncs.Domain.Model;
using System.Text.Json;
using SAK;
using Ncs.EventSourcing;
using System.Text;
using Serilog.Configuration;
using Microsoft.Extensions.Localization;
using Ncs.L10n;

namespace Ncs.Agency.Ui.Web.Controllers
{
	public record ListEntry(string StreamIdentifier, string Identity, DateTime Created, ulong Seq);
	public class HomeController : Controller
	{
		private readonly ILogger _logger;
		private readonly IEventStore _eventStore;
		private readonly IUniqueIdGenerator _uniqueIdGenerator;
		private readonly DebtCollectionClientCommandHandler _debtCollectionClientCommandHandler;
		private readonly IStringLocalizer<SharedBasicResource> _sharedBasicLocalizer;

		public HomeController(
			ILogger<HomeController> logger,
			IStringLocalizer<SharedBasicResource> sharedBasicLocalizer,
			IEventStore eventStore,
			IUniqueIdGenerator uniqueIdGenerator,
			DebtCollectionClientCommandHandler debtCollectionClientCommandHandler)
		{
			_logger = logger;
			_sharedBasicLocalizer = sharedBasicLocalizer;
			_eventStore = eventStore;
			_uniqueIdGenerator = uniqueIdGenerator;
			_debtCollectionClientCommandHandler = debtCollectionClientCommandHandler;
		}

		public async Task<ActionResult> Index()
		{
			var eventsAsync = _eventStore.ReadCategoryStreamFullAsync(DebtCollectionClientEntity.EntityName);
			var events = await eventsAsync.ToListAsync();

			List<ListEntry> result = new();
			foreach (var @event in events)
			{
				var data = Encoding.UTF8.GetString(@event.Data.ToArray());
				var parts = data.Split("@");
				var seq = parts[0];
				var streamIdent = parts[1];
				var streamIdenPairs = streamIdent.Split("-");
				var identity = streamIdenPairs[1];
				var entry = new ListEntry(streamIdent, identity, @event.Created, ulong.Parse(seq));
				if (!string.IsNullOrEmpty(entry.Identity))
				{
					result.Add(entry);
				}
			}
			return View(result);
		}

		[Route("Details/{id}")]
		public async Task<ActionResult> Details([FromRoute] string id)
		{
			var currentClient = new DebtCollectionClientEntity(id);
			var eventsAsync = _eventStore.ReadStreamFullAsync(DebtCollectionClientEntity.EntityName, id);
			var events = await eventsAsync.ToListAsync();
			foreach (var @event in events)
			{
				currentClient.Apply(@event.EventType, Encoding.UTF8.GetString(@event.Data.ToArray()));
			}
			return View(currentClient);
		}

		[HttpGet]
		public async Task<ActionResult> Create()
		{
			await Task.CompletedTask;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create([FromForm] DebtCollectionClientCreateModel_V1 modelCreate)
		{
			var entity = new DebtCollectionClientEntity(_uniqueIdGenerator.MakeOne());
			var command = new CreateDebtCollectionCommand(
				modelCreate.PersonalIdentificationNumber,
				modelCreate.Name);
			await _debtCollectionClientCommandHandler.Handle(entity, command);
			return RedirectToAction(nameof(Details), new { entity.Id });
		}

		[Route("SoftDelete/{id}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SoftDelete(string id)
		{
			var @event = new DebtCollectionClientDeletedEvent_V1(id);
			var result = await _eventStore.StoreEventAsync(@event, HttpContext.RequestAborted);
			_logger.LogInformation("Result: {result}", result.ToJson());
			return RedirectToAction(nameof(Index), new { message = _sharedBasicLocalizer["Deleted"] });
		}
	}
}
