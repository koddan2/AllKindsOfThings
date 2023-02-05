using FluentAssertions;
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
			string? initialPaymentRef;
			{
				var agg = await new CaseAggregate(caseId).Hydrate(reader: _eventReader, ExpectedStateOfStream.Absent);
				agg.Revision.Should().Be(0);
				await agg.Handle(sender: _eventSender, new CreateNewCaseCommand());
				agg.Root!.PaymentReference.Should().NotBeNullOrWhiteSpace();
				Console.WriteLine($"{agg.Root}");
				initialPaymentRef = agg.Root!.PaymentReference;
			}
			{
				var agg = await new CaseAggregate(caseId).Hydrate(reader: _eventReader);
				agg.Revision.Should().Be(1);
				agg.Root!.PaymentReference.Should().NotBeNullOrWhiteSpace();
				agg.Root!.PaymentReference.Should().Be(initialPaymentRef);
				Console.WriteLine($"{agg.Root}");
			}
			{
				var agg = await new CaseAggregate(caseId).Hydrate(reader: _eventReader);
				agg.Revision.Should().Be(1);
				await agg.Handle(_eventSender, new GenerateNewPaymentReferenceCommand());
				await agg.Hydrate(reader: _eventReader);
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