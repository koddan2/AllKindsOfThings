using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncs.Solicitor.Data.DbManagement.Processing
{
    public partial class Migrator
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IEnumerable<IMigration> _migrations;

        public Migrator(IDbConnectionFactory connection, IEnumerable<IMigration> migrations)
        {
            _connectionFactory = connection;
            _migrations = migrations;
        }

        public void Migrate()
        {
            using (var connection = _connectionFactory.GetConnection())
            {
                var count = connection.ExecuteScalar<int>(
                    @"select count(*) from information_schema.tables where table_name = @TableName",
                    new { Migration.TableName });
                if (count is 0)
                {
                    _ = connection.Execute(@"create table ncs_migration()");
                }

                var installedMigrations = connection.Query<Migration>("select * from ncs_migration");

                using var transaction = connection.BeginTransaction();
                foreach (var migration in _migrations.OrderBy(x => x.Version))
                {
                    if (installedMigrations.Any(x => x.Version == migration.Version))
                    {
                        // Already done!
                        continue;
                    }

                    migration.Run(connection);

                    _ = connection.Execute(
                        Migration.Insert.Sql,
                        new Migration.Insert
                        {
                            Version = migration.Version,
                            CreatedAt = DateTimeOffset.UtcNow,
                        });
                }

                transaction.Commit();
            }
        }
    }
}
