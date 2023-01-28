using FluentMigrator;
using Ncs.Model.Database.Core.Parties;

namespace Ncs.Agency.DataContext.Migrations
{
    [Migration(202301280010)]
    public class M202301280010_AddDebtCollectionParty : Migration
    {
        public override void Up()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionParty>();
            var table = Create
                .Table(tableMetadata.Name)
                .InSchema(tableMetadata.Schema);
            CommonDdl.ProcessCreateBaseTransactionalDatabaseModelWithIdentifiers<DebtCollectionParty>(table, tableMetadata);

            ////_ = table
            ////        ;
        }

        public override void Down()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionParty>();
            Delete
                .Table(tableMetadata.Name)
                .InSchema(tableMetadata.Schema);
        }
    }
}