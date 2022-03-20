using DatabaseSchemaReader.DataSchema;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Akot.Database.Analyzer.Cli
{
    class Application
    {
        private readonly TargetSqlConnectionStringResolver connString;
        private readonly Storage storage;

        public Application(TargetSqlConnectionStringResolver connString, Storage storage)
        {
            this.connString = connString;
            this.storage = storage;
        }

        public async Task RunAsync()
        {
            using (var connection = new SqlConnection(connString.ToString()))
            {
                var dbReader = new DatabaseSchemaReader.DatabaseReader(connection);
                //Then load the schema (this will take a little time on moderate to large database structures)
                var schema = dbReader.ReadAll();

                //The structure is identical for all providers (and the full framework).
                ////foreach (var table in schema.Tables)
                ////{
                ////    //do something with your model
                ////    Console.WriteLine("{0}.{1}", table.Tag, table.Name);
                ////}

                ////Console.WriteLine(json);
                //await File.WriteAllTextAsync("schema.json", json);

                foreach (var tab in schema.Tables)
                {
                    storage.InsertTable(tab);
                    foreach (var col in tab.Columns)
                    {
                        storage.InsertColumn(col);
                    }
                }
            }
        }

        string Serialize(DatabaseSchema schema)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            // This tells your serializer that multiple references are okay.
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            var json = JsonConvert.SerializeObject(schema, settings);
            return json;
        }
    }
}
