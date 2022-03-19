// See https://aka.ms/new-console-template for more information
using System.Data.SqlClient;

Console.WriteLine("Hello, World!");

var username = "development_user";
var password = "CCfLgzQ8MvY6kDw";
var dataSource = @"192.168.9.15\aw";
var initialCatalog = "AdventureWorks2019";
SqlConnectionStringBuilder sqlConnString = new()
{
    UserID = username,
    Password = password,
    DataSource = dataSource,
    InitialCatalog = initialCatalog,
    IntegratedSecurity = false,
    ConnectTimeout = 3,
};
using (var connection = new SqlConnection(sqlConnString.ToString()))
{
    var dbReader = new DatabaseSchemaReader.DatabaseReader(connection);
    //Then load the schema (this will take a little time on moderate to large database structures)
    var schema = dbReader.ReadAll();

    //The structure is identical for all providers (and the full framework).
    foreach (var table in schema.Tables)
    {
        //do something with your model
    }
}
