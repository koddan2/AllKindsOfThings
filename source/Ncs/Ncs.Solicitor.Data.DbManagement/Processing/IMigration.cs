using System.Data;

namespace Ncs.Solicitor.Data.DbManagement.Processing
{
    public interface IMigration
    {
        void Run(IDbConnection connection);

        int Version { get; }
    }
}
