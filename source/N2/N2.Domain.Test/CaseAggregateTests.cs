using FluentAssertions;
using N2.Domain.DebtCollectionCase;
using N2.Domain.DebtCollectionCase.Commands;
using System.Diagnostics;

namespace N2.Domain.Test
{
	public class CaseAggregateTests
	{
		private TestEventSender _eventSender;
		private TestEventReader _eventReader;

		[SetUp]
		public void Setup()
		{
			var eventLog = new TestEventLog();
			_eventSender = new TestEventSender(eventLog);
			_eventReader = new TestEventReader(eventLog);
		}

		[Test]
		public async Task Test1()
		{
			var caseId = MakeUniqueIdentityString();
			var commandHandler = new CaseAggregateCommandHandler(_eventReader, _eventSender);
			string? initialPaymentRef;
			{
				var agg = new CaseAggregate(caseId);
				agg.Revision.Should().Be(0);
				await commandHandler.Handle(agg, new CreateNewCaseCommand());
				agg.Root!.PaymentReference.Should().NotBeNullOrWhiteSpace();
				Console.WriteLine($"{agg.Root}");
				initialPaymentRef = agg.Root!.PaymentReference;
			}
			{
				var agg = new CaseAggregate(caseId);
				await commandHandler.Hydrate(agg);
				agg.Revision.Should().Be(1);
				agg.Root!.PaymentReference.Should().NotBeNullOrWhiteSpace();
				agg.Root!.PaymentReference.Should().Be(initialPaymentRef);
				Console.WriteLine($"{agg.Root}");
			}
			{
				var agg = new CaseAggregate(caseId);
				await commandHandler.Hydrate(agg);
				agg.Revision.Should().Be(1);
				await agg.Handle(_eventSender, new GenerateNewPaymentReferenceCommand());
				agg = new CaseAggregate(caseId);
				await commandHandler.Hydrate(agg);
				agg.Revision.Should().Be(2);
				agg.Root!.PaymentReference.Should().NotBeNullOrWhiteSpace();
				agg.Root!.PaymentReference.Should().NotBe(initialPaymentRef);
				Console.WriteLine($"{agg.Root}");
			}
		}

		private static string MakeUniqueIdentityString()
		{
			var uniqueValue = Guid.NewGuid().ToByteArray();
			var b64 = Convert.ToBase64String(uniqueValue);
			var result = b64
				.Replace("/", "_")
				.Replace("+", ".")
				.Replace("==", string.Empty);
			return result;
		}
	}
}