﻿using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CommitUnitOfWorkCommandHandlerDecorator<TCommand>
        : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommandHandler<TCommand> _decoratee;

        public CommitUnitOfWorkCommandHandlerDecorator(
            IUnitOfWork unitOfWork,
            ICommandHandler<TCommand> decoratee)
        {
            _unitOfWork = unitOfWork;
            _decoratee = decoratee;
        }

        public async Task ExecuteAsync(TCommand message, CancellationToken token)
        {
            await _decoratee.ExecuteAsync(message, token);
            await _unitOfWork.CommitAsync(token);
        }
    }

    //public class CommitUnitOfWorkCommandHandlerDecorator2<TCommand, TCommandResult>
    //    : ICommandHandler<TCommand, TCommandResult> where TCommand : ICommand where TCommandResult : ICommandResult
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly ICommandHandler<TCommand, TCommandResult> _decoratee;

    //    public CommitUnitOfWorkCommandHandlerDecorator2(
    //        IUnitOfWork unitOfWork,
    //        ICommandHandler<TCommand, TCommandResult> decoratee)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _decoratee = decoratee;
    //    }

    //    public async Task<TCommandResult> ExecuteAsync(TCommand message, CancellationToken token)
    //    {
    //        var result = await _decoratee.ExecuteAsync(message, token);
    //        await _unitOfWork.CommitAsync(token);
    //        return result;
    //    }
    //}
}