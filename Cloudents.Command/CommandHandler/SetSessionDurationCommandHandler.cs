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
        private readonly IRepository<StudyRoomPayment> _studyRoomSessionUseRepository;
        public SetSessionDurationCommandHandler(IRepository<StudyRoomSession> repository, IRepository<StudyRoomPayment> studyRoomSessionUseRepository)
        {
            _repository = repository;
            _studyRoomSessionUseRepository = studyRoomSessionUseRepository;
        }

        public async Task ExecuteAsync(SetSessionDurationCommand message, CancellationToken token)
        {
            var studyRoomPayment = await _studyRoomSessionUseRepository.GetAsync(message.SessionId, token);


            if (studyRoomPayment != null)
            {

                if (studyRoomPayment.Tutor.Id != message.TutorId)
                {
                    throw new ArgumentException();
                }
                studyRoomPayment.ApproveSession(message.RealDuration, message.Price);
                return;
            }

            //Old version
            var session = await _repository.LoadAsync(message.SessionId, token);
            if (session.StudyRoom.Tutor.Id != message.TutorId)
            {
                throw new ArgumentException();
            }
            session.SetRealDuration(message.RealDuration, message.Price);
            await _repository.UpdateAsync(session, token);
        }
    }
}
