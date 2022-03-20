using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Akot.Database.Analyzer.Cli
{
    class ConnString
    {
        public ConnString(IConfiguration cfg)
        {
            this.ConnectionStringBuilder = new SqlConnectionStringBuilder
            {
                UserID = cfg.GetValue<string>("Database:UserID"),
                Password = cfg.GetValue<string>("Database:Password"),
                DataSource = cfg.GetValue<string>("Database:DataSource"),
                InitialCatalog = cfg.GetValue<string>("Database:InitialCatalog"),
                IntegratedSecurity = false,
                ConnectTimeout = 3,
            };
        }

        public SqlConnectionStringBuilder ConnectionStringBuilder { get; }

        public override string ToString()
        {
            return ConnectionStringBuilder.ToString();
        }
    }
}
