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
        private readonly IRepository<StudyRoomPayment> _studyRoomSessionUseRepository;
        public SetSessionDurationCommandHandler(IRepository<StudyRoomPayment> studyRoomSessionUseRepository)
        {
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
            }

         
        }
    }
}
