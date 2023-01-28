using SAK;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Ncs.Solicitor.Data.DbManagement.Migrations
{
    internal static class Attr
    {
        public static TableAttribute GetTable<T>()
        {
            var tableMetadata = typeof(T).GetCustomAttribute<TableAttribute>().OrFail();
            return tableMetadata.OrFail();
        }
    }
}