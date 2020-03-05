using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class SetSessionDurationCommandHandler : ICommandHandler<SetSessionDurationCommand>
    {
        private readonly IRepository<StudyRoomSession> _repository;
        public SetSessionDurationCommandHandler(IRepository<StudyRoomSession> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(SetSessionDurationCommand message, CancellationToken token)
        {
            var session = await _repository.LoadAsync(message.SessionId, token);
            if (session.StudyRoom.Tutor.Id != message.UserId)
            {
                throw new ArgumentException();
            }
            session.SetRealDuration(message.RealDuration);
            await _repository.UpdateAsync(session, token);
        }
    }
}
