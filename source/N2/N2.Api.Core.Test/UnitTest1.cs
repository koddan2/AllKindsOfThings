using FluentAssertions;
using N2.Domain.DcCase;
using N2.Test.Common;

namespace N2.Api.Core.Test
{
	public class Tests
	{
		private TestEventSender _eventSender;
		private TestEventReader _eventReader;
		private CaseAggregateEventReader _caseAggregateEventReader;
		private CaseAggregateCommandHandler _caseAggregateCommandHandler;
		private N2Facade _facade;

		[SetUp]
		public void Setup()
		{
			var eventLog = new TestEventLog();
			_eventSender = new TestEventSender(eventLog);
			_eventReader = new TestEventReader(eventLog);
			_caseAggregateEventReader = new CaseAggregateEventReader(_eventReader);
			_caseAggregateCommandHandler = new CaseAggregateCommandHandler(_eventReader, _eventSender);
			_facade = new N2Facade(_caseAggregateEventReader, _caseAggregateCommandHandler);
		}

		[Test]
		[TestCase("abc")]
		[TestCase("cb47b245-a868-4052-9cf2-50f7a5fb2948")]
		[TestCase("asdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdf")]
		public async Task TestBasicStuff1(string caseId)
		{
			await _facade.DcCaseCreate(new DcCaseViewModelCreate(caseId, "cde"));
			var log = await _facade.DcCaseGetLogSingle(caseId);
			log.Should().NotBeNull();
			log.Count().Should().Be(1);
		}
	}
}