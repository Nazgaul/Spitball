﻿using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Infrastructure.CommandHandlers
{
    public interface ICommandBus
    {
        TCommandResult Dispatch<TCommand, TCommandResult>(TCommand command)
                                                          where TCommand : ICommand
                                                          where TCommandResult : ICommandResult;

        TCommandResult Dispatch<TCommand, TCommandResult>(TCommand command, string name)
            where TCommand : ICommand
            where TCommandResult : ICommandResult;

        void Send<TCommand>(TCommand command) where TCommand : ICommand;
        //void SendGeneric(ICommand command);

        Task SendAsync<TCommand>(TCommand command) where TCommand : ICommandAsync;

        Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command)
            where TCommand : ICommandAsync
            where TCommandResult : ICommandResult;

        Task<TCommandResult> DispatchAsync<TCommand, TCommandResult>(TCommand command, string name)
            where TCommand : ICommandAsync
            where TCommandResult : ICommandResult;
    }
}
