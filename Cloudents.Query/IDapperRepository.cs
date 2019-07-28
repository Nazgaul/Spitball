using System.Data;

namespace Cloudents.Query
{
    public interface IDapperRepository
    {
        IDbConnection OpenConnection();
    }
}