
namespace Zbang.Zbox.WorkerRole.Jobs
{
    public interface IJob
    {
        void Run();
        void Stop();
    }
}
