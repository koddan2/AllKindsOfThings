using DatabaseSchemaReader.DataSchema;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Akot.Database.Analyzer.Cli
{
    class App
    {
        private readonly ConnString connString;

        public App(ConnString connString)
        {
            this.connString = connString;
        }

        public async Task RunAsync()
        {
            using (var connection = new SqlConnection(connString.ToString()))
            {
                var dbReader = new DatabaseSchemaReader.DatabaseReader(connection);
                //Then load the schema (this will take a little time on moderate to large database structures)
                var schema = dbReader.ReadAll();

                Console.WriteLine(schema.Provider);
                //The structure is identical for all providers (and the full framework).
                ////foreach (var table in schema.Tables)
                ////{
                ////    //do something with your model
                ////    Console.WriteLine("{0}.{1}", table.Tag, table.Name);
                ////}

                ////Console.WriteLine(json);
                //await File.WriteAllTextAsync("schema.json", json);
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
