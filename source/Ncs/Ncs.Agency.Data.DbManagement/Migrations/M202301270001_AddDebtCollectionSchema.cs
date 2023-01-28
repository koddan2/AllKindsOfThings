using FluentMigrator;
using Ncs.Model.Database.Core;

namespace Ncs.Solicitor.Data.DbManagement.Migrations
{
    [Migration(202301270001)]
    public class M202301270001_AddDebtCollectionSchema : Migration
    {
        public override void Up()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionClaimAccount>();
            _ = Create.Schema(tableMetadata.Schema);
        }
        public override void Down()
        {
            var tableMetadata = Attr.GetTable<DebtCollectionClaimAccount>();
            Delete.Schema(tableMetadata.Schema);
        }
    }
}