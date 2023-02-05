using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N2.Domain.DcCase;
using N2.EventSourcing;

namespace N2.Api.Core.Test.Integration
{
	public class Tests
	{
		private IHost _app;

		[SetUp]
		public void Setup()
		{
			var hostBuilder = Host.CreateDefaultBuilder();
			hostBuilder.ConfigureAppConfiguration(builder =>
			{
				builder.AddIniFile("appsettings.ini");
			});
			hostBuilder.ConfigureServices((context, services) =>
			{
				services.AddN2ApiApplication(context.Configuration.GetSection("N2"));
			});
			_app = hostBuilder.Build();
		}

		[Test]
		public async Task Test1()
		{
			using var scope = _app.Services.CreateAsyncScope();
			var facade = scope.ServiceProvider.GetRequiredService<N2Facade>();

			var caseId = Guid.NewGuid().ToString("N");
			await facade.DcCaseCreate(new DcCaseViewModelCreate(caseId, ""));
			await facade.DcCaseGenerateNewPaymentRef(caseId);
			await facade.DcCaseGenerateNewPaymentRef(caseId);
			await facade.DcCaseGenerateNewPaymentRef(caseId);

			{
				var events = await facade.DcCaseGetLogSingle(caseId);
				events.Count().Should().Be(4);
			}
		}
	}
}