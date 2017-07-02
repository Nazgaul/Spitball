using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    public static class TaskExtensions
    {
        public static readonly Task<bool> CompletedTaskTrue = Task.FromResult(true);
        public static readonly Task<bool> CompletedTaskFalse = Task.FromResult(false);
        public static readonly Task<string> CompletedTaskString = Task.FromResult<string>(null);
    }
}
