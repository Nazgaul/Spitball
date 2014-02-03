﻿using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.CommandHandlers
{
    public class CommandBus : ICommandBus
    {
        private readonly IocFactory m_Container = IocFactory.Unity;

        public TCommandResult Dispatch<TCommand, TCommandResult>(TCommand command)
            where TCommand : Commands.ICommand
            where TCommandResult : Commands.ICommandResult
        {

            return m_Container.Resolve<ICommandHandler<TCommand, TCommandResult>>().Execute(command);

        }

        public TCommandResult Dispatch<TCommand, TCommandResult>(TCommand command, string name)
            where TCommand : Commands.ICommand
            where TCommandResult : Commands.ICommandResult
        {

            return m_Container.Resolve<ICommandHandler<TCommand, TCommandResult>>(name).Execute(command);
        }

        public void Send<TCommand>(TCommand command) where TCommand : Commands.ICommand
        {
            m_Container.Resolve<ICommandHandler<TCommand>>().Handle(command);
        }
    }
}
