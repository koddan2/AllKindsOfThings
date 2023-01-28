﻿using Npgsql;
using System.Data;

namespace Ncs.Solicitor.Data.DbManagement.Processing
{
    public class PostgresDbConnectionFactory : IDbConnectionFactory
    {
        IDbConnection IDbConnectionFactory.GetConnection()
        {
            var conn = new NpgsqlConnection("Host=localhost;Username=ncsusr;Password=abc123;Database=postgres;");
            conn.Open();
            return conn;
        }
    }
}
