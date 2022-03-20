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
            LoadSqlProperties<DatabaseTable>();
            LoadSqlProperties<DatabaseColumn>();
        }

        private static void LoadSqlProperties<T>()
        {
            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            TypeProps[typeof(T)] = props;
        }

        static IDictionary<Type, PropertyInfo[]> TypeProps { get; } = new Dictionary<Type, PropertyInfo[]>();

        private void Migrate()
        {
            Cmd.CommandText = "PRAGMA user_version;";
            var version = Convert.ToInt64(Cmd.ExecuteScalar());
            if (version == 0)
            {
                CreateTable<DatabaseTable>("target_table");
                CreateTable<DatabaseColumn>("target_column");
            }
        }

        private void CreateTable<T>(string tabName)
        {
            var columns = string.Join(",", TypeProps[typeof(T)].OrderBy(p => p.Name).Select(p => $"\"{p.Name}\" text null"));
            var sqlTemplate = GetSql("migration-0.template.sql");
            Cmd.CommandText = String.Format(
                sqlTemplate,
                $"\"{tabName}\"",
                columns);
            Cmd.ExecuteNonQuery();
        }

        private string GetSql(string fileName)
        {
            return File.ReadAllText(Path.Combine("Sql", fileName));
        }

        private SqliteConnection Db { get; }
        private SqliteCommand Cmd { get; }

        class SqlInsertionMetadata
        {
            public string ColumnNameList { get; set; }
            public string ParameterNameList { get; set; }
        }
        SqlInsertionMetadata GetSqlInsertionMetadata<T>()
        {
            var columns = string.Join(",", TypeProps[typeof(T)].Select(p => $"\"{p.Name}\""));
            var paramNameList = string.Join(",", TypeProps[typeof(T)].Select(p => $"@{p.Name}"));
            return new SqlInsertionMetadata
            {
                ColumnNameList = columns,
                ParameterNameList = paramNameList,
            };
        }
        SqliteParameter[] GetParameters<T>(T instance)
        {
            var parameters = TypeProps[typeof(T)]
                .Select(prop =>
                {

                    var value = prop.GetGetMethod()?.Invoke(instance, null);
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
            return parameters;
        }

        internal void InsertRecord<T>(T instance, string tabName)
        {
            var meta = GetSqlInsertionMetadata<T>();
            var parameters = GetParameters(instance);
            var sql = $"insert into {tabName} ({meta.ColumnNameList}) values ({meta.ParameterNameList})";
            DoCommandDirect(sql, parameters);
        }

        internal void InsertTable(DatabaseTable tab)
        {
            InsertRecord(tab, "target_table");
        }

        internal void InsertColumn(DatabaseColumn col)
        {
            InsertRecord(col, "target_column");
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
