// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using System.Text.Json;
using System.Threading;

namespace Ncs.Explore.Cli;

public static class Testing
{
    public static void handler(DeliveryReport<Null, string> dr)
    {
        Console.WriteLine(dr.Value);
    }

    internal static void Consume1()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:29092",
            GroupId = "grp.2",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromSeconds(8));
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        //while (!cancelled)
        var topics = new[] { "weblog" }; 
        consumer.Subscribe(topics);
        while (!cts.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(cancellationToken: cts.Token);

            // handle consumed message.
            var str= JsonSerializer.Serialize(consumeResult, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(str);
        }

        consumer.Close();
    }

    internal static async Task Produce1()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:29092",
        };

        using var producer = new ProducerBuilder<Null, string>(config).Build();
        var result = await producer.ProduceAsync("weblog", new Message<Null, string> { Value = "a log message" });
        producer.Produce("weblog", new Message<Null, string> { Value = "a log message" }, handler);
    }
}