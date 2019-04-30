using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class CreateStudyRoomCommandHandler : ICommandHandler<CreateStudyRoomCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IGoogleDocument _googleDocument;

        public CreateStudyRoomCommandHandler(IRegularUserRepository userRepository, IRepository<StudyRoom> studyRoomRepository, IGoogleDocument googleDocument)
        {
            _userRepository = userRepository;
            _studyRoomRepository = studyRoomRepository;
            _googleDocument = googleDocument;
        }

        public async Task ExecuteAsync(CreateStudyRoomCommand message, CancellationToken token)
        {
            var userTutor = await _userRepository.LoadAsync(message.TutorId, token);
            if (userTutor.Tutor == null)
            {
                throw new InvalidOperationException("user is not a tutor");
            }
            var student = await _userRepository.LoadAsync(message.StudentId, token);
            var url = await _googleDocument.CreateOnlineDocAsync(
                ChatRoom.BuildChatRoomIdentifier(new[] {userTutor.Id, student.Id}), token);
            var studyRoom = new StudyRoom(userTutor.Tutor, student,url);
            await _studyRoomRepository.AddAsync(studyRoom, token);


        }
    }
}