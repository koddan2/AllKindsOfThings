// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Ncs.Explore.Cli.KafkaTest;
internal class KafkaTestProducer1
{
    private readonly Random _rnd = new();
    private readonly ILogger _log;

    public KafkaTestProducer1(ILogger log)
    {
        _log = log;
    }

    internal void Run(IEnumerable<KeyValuePair<string,string?>> configuration)
    {
        const string topic = "purchases";

        string[] users = { "eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther" };
        string[] items = { "book", "alarm clock", "t-shirts", "gift card", "batteries" };

        using var producer = new ProducerBuilder<string, string>(
            configuration.AsEnumerable()).Build();
        var numProduced = 0;
        const int numMessages = 10;
        for (int i = 0; i < numMessages; ++i)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            var user = users[_rnd.Next(users.Length)];
            var item = items[_rnd.Next(items.Length)];

            producer.Produce(topic, new Message<string, string> { Key = user, Value = item },
                (deliveryReport) =>
                {
                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                    {
                        _log.Error("Failed to deliver message: {report}", deliveryReport);
                    }
                    else
                    {
                        _log.Information("Produced event to topic {topic}: {@message}", topic, deliveryReport.Message);
                        numProduced += 1;
                    }
                });
        }

        producer.Flush(TimeSpan.FromSeconds(10));
        _log.Information("{numProduced} messages were produced to topic {topic}", numProduced, topic);
    }
}
