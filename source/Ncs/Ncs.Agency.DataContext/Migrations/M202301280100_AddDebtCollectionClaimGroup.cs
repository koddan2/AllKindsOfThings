using FluentMigrator;
using Ncs.Model.Database.Core;
using Ncs.Model.Database.Core.Parties;

namespace Ncs.Agency.DataContext.Migrations
{
    [Migration(202301280100)]
    public class M202301280100_AddDebtCollectionClaimGroup : Migration
    {
        public override void Up()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionClaimGroup>();
            var table = Create
                .Table(tableMetadata.Name)
                .InSchema(tableMetadata.Schema);
            CommonDdl.ProcessCreateBaseTransactionalDatabaseModelWithIdentifiers<DebtCollectionClaimGroup>(table, tableMetadata);

            _ = table
                .WithColumn("reference_code")
                    .AsString().NotNullable()
                .WithColumn("claimant_party_id")
                    .AsInt64().NotNullable()
                    ;

            {
                var partyTableMetadata = Attr.GetTable<DebtCollectionParty>();
                _ = Create.ForeignKey("FK_ClaimGroup_Party")
                    .FromTable(tableMetadata.Name)
                        .InSchema(tableMetadata.Schema)
                        .ForeignColumn("claimant_party_id")
                    .ToTable(partyTableMetadata.Name)
                        .InSchema(partyTableMetadata.Schema)
                        .PrimaryColumn("id");
            }
        }

        public override void Down()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionClaimGroup>();
            Delete.ForeignKey("FK_ClaimGroup_Party")
                .OnTable(tableMetadata.Name)
                .InSchema(tableMetadata.Schema);
            Delete
                .Table(tableMetadata.Name)
                .InSchema(tableMetadata.Schema);
        }
    }
}