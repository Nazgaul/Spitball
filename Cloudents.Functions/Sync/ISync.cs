using System.Threading.Tasks;
using Cloudents.Core.Query;

namespace Cloudents.Functions.Sync
{
    public interface IDbToSearchSync
    {
        SyncAzureQuery GetCurrentState();

        Task CreateIndex();


        Task DoSync();
        //Task<(IEnumerable<T> update, IEnumerable<long> delete, long version)>GetData<T>();

    }

    //public class QuestionDbToSearchSync : IDbToSearchSync
    //{


        

    //    //public SyncAzureQuery GetCurrentState()
    //    //{
    //    //    throw new System.NotImplementedException();
    //    //}

    //    //public Task CreateIndex()
    //    //{
    //    //    throw new System.NotImplementedException();
    //    //}

    //    //public Task<(IEnumerable<Question> update, IEnumerable<long> delete, long version)> GetData<Question>()
    //    //{
    //    //    throw new System.NotImplementedException();
    //    //}

    //    //public Task<(IEnumerable<Question> update, IEnumerable<long> delete, long version)> GetData()
    //    //{
    //    //    throw new System.NotImplementedException();
    //    //}
    //}
}