using System.Threading.Tasks;
using Autofac;

namespace Zbang.Zbox.Infrastructure.CommandHandlers
{
    public class CommandBus : ICommandBus
    {
        public CommandBus(ILifetimeScope container)
        {
            m_Container = container;
        }

        private readonly ILifetimeScope m_Container;// = IocFactory.Unity;

        public TCommandResult Dispatch<TCommand, TCommandResult>(TCommand command)
            where TCommand : Commands.ICommand
            where TCommandResult : Commands.ICommandResult
        {
            return m_Container.Resolve<ICommandHandler<TCommand, TCommandResult>>().Execute(command);
        }

        public Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command)
            where TCommand : Commands.ICommandAsync
            where TCommandResult : Commands.ICommandResult
        {
            return m_Container.Resolve<ICommandHandlerAsync<TCommand, TCommandResult>>().ExecuteAsync(command);
        }
        public Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, string name)
            where TCommand : Commands.ICommandAsync
            where TCommandResult : Commands.ICommandResult
        {
            return m_Container.ResolveNamed<ICommandHandlerAsync<TCommand, TCommandResult>>(name).ExecuteAsync(command);
        }


        public TCommandResult Dispatch<TCommand, TCommandResult>(TCommand command, string name)
            where TCommand : Commands.ICommand
            where TCommandResult : Commands.ICommandResult
        {

            return m_Container.ResolveNamed<ICommandHandler<TCommand, TCommandResult>>(name).Execute(command);
        }

        public void Send<TCommand>(TCommand command) where TCommand : Commands.ICommand
        {
            m_Container.Resolve<ICommandHandler<TCommand>>().Handle(command);
        }

        //public void SendGeneric(Commands.ICommand command)
        //{
        //    var z = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        //    var type = m_Container.Resolve<ICommandHandler<Commands.ICommand>>(z);
        //    type.Handle(command);

        //    //m_Container.Resolve<ICommandHandler<TCommand>>().Handle(command);
        //}

        public Task SendAsync<TCommand>(TCommand command) where TCommand : Commands.ICommandAsync
        {
            return m_Container.Resolve<ICommandHandlerAsync<TCommand>>().HandleAsync(command);
        }


       
    }
}
