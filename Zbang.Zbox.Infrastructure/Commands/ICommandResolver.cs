
namespace Zbang.Zbox.Infrastructure.Commands
{
    public interface ICommandResolver : ICommand
    {
        string ResolverName { get; }
    }
}
