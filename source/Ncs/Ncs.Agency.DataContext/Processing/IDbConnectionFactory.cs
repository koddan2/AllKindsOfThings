using System.Data;

namespace Ncs.Agency.DataContext.Processing
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
