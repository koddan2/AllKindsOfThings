using FluentMigrator;

namespace Ncs.Solicitor.Data.DbManagement.Migrations
{
    [Migration(202301280001)]
    public class AddDebtCollectionCase : Migration
    {
        public override void Up()
        {
            _ = Create.Table("ncs_debtcollection_case")
                .WithColumn("id")
                    .AsInt64().PrimaryKey().Identity()
                .WithColumn("unique_id")
                    .AsGuid().NotNullable()
                .WithColumn("created_at")
                    .AsDateTimeOffset().NotNullable();
                ;
        }

        public override void Down()
        {
            _ = Delete.Table("ncs_debtcollection_case");
        }
    }
}