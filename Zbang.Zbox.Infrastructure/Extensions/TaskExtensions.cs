using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    public static class TaskExtensions
    {
        //private static readonly Task completedTask = CompletedTaskFalse;


        public static readonly Task<bool> CompletedTaskTrue = Task.FromResult(true);
        public static readonly Task<bool> CompletedTaskFalse = Task.FromResult(false);
        public static readonly Task<string> CompletedTaskString = Task.FromResult<string>(null);

        //public static Task CompletedTask => CompletedTaskFalse;
    }
}
