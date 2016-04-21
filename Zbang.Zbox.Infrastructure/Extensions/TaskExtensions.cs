using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Extensions
{
    public static class TaskExtensions
    {
        public static readonly Task CompletedTask = Task.FromResult(false);
    }
}
