using Microsoft.Data.Sqlite;
using SqlKata.Execution;

internal class DatabaseMigrator
{
    private readonly QueryFactory db;

    public DatabaseMigrator(QueryFactory db)
    {
        this.db = db;
    }

    internal void Migrate()
    {
        var cmd = db.Connection.CreateCommand();
        cmd.CommandText = "PRAGMA user_version;";
        var userVersion = cmd.ExecuteScalar();
        if (userVersion?.ToString() == "0")
        {
            cmd.CommandText = File.ReadAllText(Path.Combine("Sql", "Migrations", "v1.sql"));
            _ = cmd.ExecuteNonQuery();
        }
        else if (userVersion?.ToString() == "1")
        {
            // next version.
        }
        else
        {
            throw new ApplicationException("Unknown user_version.");
        }
    }
}