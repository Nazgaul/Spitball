﻿using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands.Quiz
{
    public class DeleteAnswerCommand : ICommand
    {
        public DeleteAnswerCommand(long userId, Guid id)
        {
            UserId = userId;
            Id = id;
        }
        public long UserId { get; private set; }
        public Guid Id { get; set; }
    }
}
