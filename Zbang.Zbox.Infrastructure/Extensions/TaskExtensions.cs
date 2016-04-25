using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    public static class TaskExtensions
    {
        //private static readonly Task completedTask = CompletedTaskFalse;


        public static readonly Task<bool> CompletedTaskTrue = Task.FromResult(true);
        public static readonly Task<bool> CompletedTaskFalse = Task.FromResult(false);

        public static Task CompletedTask => CompletedTaskFalse;
    }
}
