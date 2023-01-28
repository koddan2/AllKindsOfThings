using System.Data;

namespace Ncs.Solicitor.Data.DbManagement.Processing
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
