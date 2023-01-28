using FluentMigrator;
using Ncs.Model.Database.Core;

namespace Ncs.Solicitor.Data.DbManagement.Migrations
{
    [Migration(202301280001)]
    public class M202301280001_AddDebtCollectionCase : Migration
    {
        public override void Up()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionClaimAccount>();
            var table = Create.Table(tableMetadata.Name);
            if (tableMetadata.Schema is string schema)
            {
                _ = table.InSchema(schema);
            }

            _ = table
            .WithColumn("id")
                .AsInt64().PrimaryKey().Identity()
            .WithColumn("unique_id")
                .AsGuid().Unique().NotNullable()
            .WithColumn("created_at")
                .AsDateTimeOffset().NotNullable();
            ;
        }

        public override void Down()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionClaimAccount>();
            var table = Delete.Table(tableMetadata.Name);
            if (tableMetadata.Schema is string schema)
            {
                table.InSchema(schema);
            }
        }
    }
}