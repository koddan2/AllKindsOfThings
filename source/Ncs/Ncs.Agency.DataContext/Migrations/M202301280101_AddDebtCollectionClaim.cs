using FluentMigrator;
using Ncs.Model.Database.Core;

namespace Ncs.Agency.DataContext.Migrations
{
    [Migration(202301280101)]
    public class M202301280101_AddDebtCollectionClaim : Migration
    {
        public override void Up()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionClaim>();
            var table = Create
                .Table(tableMetadata.Name)
                .InSchema(tableMetadata.Schema);
            CommonDdl.ProcessCreateBaseTransactionalDatabaseModelWithIdentifiers<DebtCollectionClaim>(table, tableMetadata);

            _ = table
                .WithColumn("claim_group_id")
                    .AsInt64().NotNullable()
                .WithColumn("claim_number")
                    .AsString().NotNullable()
                .WithColumn("description")
                    .AsString().NotNullable()
                .WithColumn("claim_date")
                    .AsDateTimeOffset().NotNullable()
                .WithColumn("due_date")
                    .AsDateTimeOffset().NotNullable()
                    ;

            {
                var claimGroupTableMetadata = Attr.GetTable<DebtCollectionClaimGroup>();
                _ = Create.ForeignKey("FK_Claim_ClaimGroup")
                    .FromTable(tableMetadata.Name)
                        .InSchema(tableMetadata.Schema)
                        .ForeignColumn("claim_group_id")
                    .ToTable(claimGroupTableMetadata.Name)
                        .InSchema(claimGroupTableMetadata.Schema)
                        .PrimaryColumn("id");
            }
        }

        public override void Down()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionClaim>();
            Delete.ForeignKey("FK_Claim_ClaimGroup")
                .OnTable(tableMetadata.Name)
                .InSchema(tableMetadata.Schema);
            Delete
                .Table(tableMetadata.Name)
                .InSchema(tableMetadata.Schema);
        }
    }
}