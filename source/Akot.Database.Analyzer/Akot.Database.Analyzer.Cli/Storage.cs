using DatabaseSchemaReader.DataSchema;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Collections;

namespace Akot.Database.Analyzer.Cli
{
    class Storage
    {
        public Storage(IConfiguration cfg)
        {
            var dbFilePath = cfg.GetValue<string>("Sqlite:FilePath");
            if (File.Exists(dbFilePath))
            {
                File.Delete(dbFilePath);
            }

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = dbFilePath,
                Mode = SqliteOpenMode.ReadWriteCreate,
            };
            this.Db = new SqliteConnection(builder.ToString());
            this.Db.Open();
            this.Cmd = this.Db.CreateCommand();

            Cmd.CommandText = "PRAGMA synchronous = 0;";
            Cmd.ExecuteNonQuery();

            this.Migrate();
        }

        static Storage()
        {
            var props = typeof(DatabaseTable).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            TypeProps[typeof(DatabaseTable)] = props;
        }
        static IDictionary<Type, PropertyInfo[]> TypeProps { get; } = new Dictionary<Type, PropertyInfo[]>();

        private void Migrate()
        {
            Cmd.CommandText = "PRAGMA user_version;";
            var version = Convert.ToInt64(Cmd.ExecuteScalar());
            if (version == 0)
            {
                var columns = string.Join(",", TypeProps[typeof(DatabaseTable)].Select(p => $"{p.Name} text null"));
                var sqlTemplate = GetSql("migration-0.sql");
                Cmd.CommandText = String.Format(sqlTemplate, columns);
                Cmd.ExecuteNonQuery();
            }
        }

        private string GetSql(string fileName)
        {
            return File.ReadAllText(Path.Combine("Sql", fileName));
        }

        private SqliteConnection Db { get; }
        private SqliteCommand Cmd { get; }

        internal void InsertTable(DatabaseTable tab)
        {
            var columns = string.Join(",", TypeProps[typeof(DatabaseTable)].Select(p => $"{p.Name}"));
            var paramNameList = string.Join(",", TypeProps[typeof(DatabaseTable)].Select(p => $"@{p.Name}"));
            var parameters = TypeProps[typeof(DatabaseTable)]
                .Select(prop =>
                {

                    var value = prop.GetGetMethod()?.Invoke(tab, null);
                    var valueType = value?.GetType();
                    if (value is IList && value.GetType().IsGenericType)
                    {
                        var listValue = (IList)value;
                        var rendering = "";
                        foreach (var item in listValue)
                        {
                            rendering += $"{item}|";
                        }
                        return new SqliteParameter($"@{prop.Name}", rendering);
                    }
                    else
                    {
                        return new SqliteParameter($"@{prop.Name}", value?.ToString() ?? (object)DBNull.Value);
                    }
                })
                .ToArray();
            var sql = $"insert into target_table ({columns}) values ({paramNameList})";
            DoCommandDirect(sql, parameters);
        }

        private void DoCommandDirect(string sql, params SqliteParameter[] sqliteParameters)
        {
            try
            {
                Cmd.Parameters.Clear();
                Cmd.CommandText = sql;
                Cmd.Parameters.AddRange(sqliteParameters);
                Cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void DoCommand(string fileName, params SqliteParameter[] sqliteParameters)
        {
            try
            {
                Cmd.Parameters.Clear();
                Cmd.CommandText = GetSql(fileName);
                Cmd.Parameters.AddRange(sqliteParameters);
                Cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
