using FluentMigrator;
using Ncs.Model.Database;

namespace Ncs.Agency.DataContext.Migrations
{
    [Migration(202301280001)]
    public class M202301280001_AddDebtCollectionSchema : Migration
    {
        public override void Up()
        {
            _ = Create.Schema(Schemas.CoreDebtCollection);
        }
        public override void Down()
        {
            Delete.Schema(Schemas.CoreDebtCollection);
        }
    }
}