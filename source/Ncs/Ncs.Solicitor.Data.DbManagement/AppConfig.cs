internal class AppConfig
{
    public PostgresConfig? Postgres { get; set; }
}
internal class PostgresConfig
{
    public string? Host { get; set; }
    public string? Database { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}