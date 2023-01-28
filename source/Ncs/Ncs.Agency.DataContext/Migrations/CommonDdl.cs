using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Ncs.Model.Database.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ncs.Agency.DataContext.Migrations
{
    internal static class CommonDdl
    {
        internal static void ProcessCreateBaseTransactionalDatabaseModelWithIdentifiers<T>(ICreateTableWithColumnSyntax table, TableAttribute tableMetadata)
            where T : BaseTransactionalDatabaseModelWithIdentifiers
        {

            _ = table
            .WithColumn("id")
                .AsInt64().PrimaryKey().Identity()
            .WithColumn("unique_id")
                .AsGuid().Unique().NotNullable()
                ;

            ProcessCreateBaseTransactionalDatabaseModel<T>(table, tableMetadata);
        }

        internal static void ProcessCreateBaseTransactionalDatabaseModel<T>(ICreateTableWithColumnSyntax table, TableAttribute tableMetadata)
            where T : BaseTransactionalDatabaseModel
        {
            _ = table
            .WithColumn("created_at")
                .AsDateTimeOffset().NotNullable()
            .WithColumn("created_by_user_identifier")
                .AsString().NotNullable()
            .WithColumn("updated_at")
                .AsDateTimeOffset().Nullable()
            .WithColumn("updated_by_user_identifier")
                .AsString().Nullable()
                ;
        }
    }
}