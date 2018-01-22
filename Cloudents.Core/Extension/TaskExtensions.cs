using System.Threading.Tasks;

namespace Cloudents.Core.Extension
{
    public static class TaskCompleted
    {
        public static readonly Task<bool> CompletedTaskTrue = Task.FromResult(true);
        public static readonly Task<bool> CompletedTaskFalse = Task.FromResult(false);
        public static readonly Task<string> CompletedTaskString = Task.FromResult<string>(null);
    }
}
