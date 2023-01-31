using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Ncs.Explore.Cli.KafkaTest
{
	internal class KafkaTestRunner
	{
		public static void RunKafkaTest1(ServiceProvider serviceProvider)
		{
			var t1 = new Thread(() =>
			{
				try
				{
					using var scope = serviceProvider.CreateScope();
					var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
					var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

					var consumer = scope.ServiceProvider.GetRequiredService<KafkaTestConsumer1>();
					consumer.Run(cfg.GetSubSectionOnly("KafkaTest.1").Where(kvp => kvp.Value is not null));
				}
				catch (Exception e)
				{
					Log.Error(e, "Unexpected!");
					throw;
				}
			});
			var t2 = new Thread(() =>
			{
				try
				{
					using var scope = serviceProvider.CreateScope();
					var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
					var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

					var producer = scope.ServiceProvider.GetRequiredService<KafkaTestProducer1>();
					producer.Run(cfg.GetSubSectionOnly("KafkaTest.1").Where(kvp => kvp.Value is not null));
				}
				catch (Exception e)
				{
					Log.Error(e, "Unexpected!");
					throw;
				}
			});

			t1.Start();
			t2.Start();

			t1.Join();
			t2.Join();
		}

		public static void RunKafkaTest2(ServiceProvider serviceProvider)
		{
			try
			{
				using var scope = serviceProvider.CreateScope();
				var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
				var appConfig = scope.ServiceProvider.GetRequiredService<IOptions<AppConfig>>();

				var consumer = scope.ServiceProvider.GetRequiredService<KafkaTestConsumer1>();
				consumer.Run(cfg.GetSubSectionOnly("KafkaTest.3").Where(kvp => kvp.Value is not null));
			}
			catch (Exception e)
			{
				Log.Error(e, "Unexpected!");
				throw;
			}
		}
	}
}
