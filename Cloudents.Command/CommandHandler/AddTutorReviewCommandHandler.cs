﻿using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddTutorReviewCommandHandler : ICommandHandler<AddTutorReviewCommand>
    {
        private readonly IRegularUserRepository _regularUserRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;

        public AddTutorReviewCommandHandler(IRegularUserRepository regularUserRepository,
                IRepository<StudyRoom> studyRoomRepository)
        {
            _regularUserRepository = regularUserRepository;
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(AddTutorReviewCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.RoomId, token);
            var tutor = studyRoom.Tutor;

            if (studyRoom.Users.Any(a => a.User.Id == message.UserId))
            {
                var user = await _regularUserRepository.LoadAsync(message.UserId, token);
                tutor.AddReview(message.Review, message.Rate, user);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
