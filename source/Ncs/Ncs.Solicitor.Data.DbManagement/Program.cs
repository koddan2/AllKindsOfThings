// See https://aka.ms/new-console-template for more information

////using Microsoft.EntityFrameworkCore;

////namespace Ncs.Solicitor.Data.DbManagement;

////public class NcsSolicitorDataContextForMigrations : NcsSolicitorDataContext
////{
////    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
////    {
////        base.OnConfiguring(optionsBuilder);
////        _ = optionsBuilder.UseNpgsql("Host=localhost;Username=ncsusr;Password=abc123;Database=postgres;");
////    }
////}

using Ncs.Solicitor.Data.DbManagement.Processing;

var factory = new PostgresDbConnectionFactory();

var migrator = new Migrator(factory, new List<IMigration>
{
   new TestMigration1(),
   new TestMigration2()
});

migrator.Migrate();
