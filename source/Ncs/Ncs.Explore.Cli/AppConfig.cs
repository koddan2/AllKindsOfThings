namespace Ncs.Explore.Cli;
internal class AppConfig
{
    public KafkaConfig? Kafka { get; set; }
}
internal class KafkaConfig
{
    public string? HostUrls { get; set; }
}