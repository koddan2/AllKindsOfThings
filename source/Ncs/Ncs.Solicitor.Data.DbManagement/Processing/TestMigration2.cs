using System.Data;

namespace Ncs.Solicitor.Data.DbManagement.Processing
{
    public class TestMigration2 : IMigration
    {
        public void Run(IDbConnection connection)
        {
            // Raw and auditable SQL.
            // Create tables, add/drop columns, etc.
        }

        public int Version => 2;
    }
}
