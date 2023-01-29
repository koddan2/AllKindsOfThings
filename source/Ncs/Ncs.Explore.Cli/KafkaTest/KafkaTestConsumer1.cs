// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Serilog;

namespace Ncs.Explore.Cli.KafkaTest;

internal class KafkaTestConsumer1
{
    private readonly CancellationTokenSource _cts = new ();
    private readonly ILogger _log;

    public KafkaTestConsumer1(ILogger log)
    {
        _log = log;
    }

    internal void Run(IEnumerable<KeyValuePair<string,string?>> configuration)
    {
        const string topic = "purchases";

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true; // prevent the process from terminating.
            _cts.Cancel();
        };

        using var consumer = new ConsumerBuilder<string, string>(configuration).Build();
        consumer.Subscribe(topic);
        try
        {
            while (true)
            {
                _cts.CancelAfter(TimeSpan.FromSeconds(8));
                Thread.Sleep(TimeSpan.FromSeconds(1));
                var cr = consumer.Consume(_cts.Token);
                _log.Information("Consumed event from topic {topic} with key {key} => {@message}", topic, cr.Message.Key, cr.Message);
                _cts.TryReset();
            }
        }
        catch (OperationCanceledException)
        {
            // Ctrl-C was pressed.
            _log.Information("Shutdown requested");
        }
        finally
        {
            consumer.Close();
        }
    }
}