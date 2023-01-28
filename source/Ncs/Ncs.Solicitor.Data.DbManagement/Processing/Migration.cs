namespace Ncs.Solicitor.Data.DbManagement.Processing
{
    public class Migration
    {
        public static string TableName => "ncs_migration";

        public class Insert
        {
            public static string Sql => $"insert into {TableName} values (@Version, @CreatedAt)";

            public int Version { get; set; }

            public DateTimeOffset CreatedAt { get; set; }
        }

        public int Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
