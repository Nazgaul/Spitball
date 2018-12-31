﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Item.Commands.FlagItem
{
    public abstract class BaseFlagItemCommandHandler<T, TId> where T : ItemObject
    {
        private readonly IRepository<T> _repository;
        private readonly IRepository<RegularUser> _userRepository;

        protected BaseFlagItemCommandHandler(IRepository<T> repository, IRepository<RegularUser> userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }
        protected async Task ExecuteAsync(BaseFlagItemCommand<TId> message, CancellationToken token)
        {
            if (!ItemObject.ValidateFlagReason(message.FlagReason))
            {
                throw new ArgumentException("reason is too long");
            }
            var answer = await _repository.LoadAsync(message.Id, token);
            User user = null;
            if (message.UserId.HasValue)
            {
                user = await _userRepository.LoadAsync(message.UserId, token);
                if (!Privileges.CanFlag(user.Score))
                {
                    throw new NoEnoughScoreException();
                }
                Validate(answer, user);
            }

            answer.Flag(message.FlagReason, user);
            await _repository.UpdateAsync(answer, token);
        }

        protected abstract void Validate(T obj, User user);


    }
}