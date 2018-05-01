namespace Cloudents.Infrastructure.Search.Job
{
    public interface IJobProvider
    {
        System.Threading.Tasks.Task<Core.DTOs.ResultWithFacetDto<Core.DTOs.JobProviderDto>> SearchAsync(JobProviderRequest jobProviderRequest, System.Threading.CancellationToken token);
    }
}