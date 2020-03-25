using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateStudyRoomCommandHandler : ICommandHandler<CreateStudyRoomCommand, CreateStudyRoomCommandResult>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IGoogleDocument _googleDocument;

        public CreateStudyRoomCommandHandler(IRegularUserRepository userRepository,
            IRepository<StudyRoom> studyRoomRepository, IGoogleDocument googleDocument
            )
        {
            _userRepository = userRepository;
            _studyRoomRepository = studyRoomRepository;
            _googleDocument = googleDocument;
        }

        public async Task<CreateStudyRoomCommandResult> ExecuteAsync(CreateStudyRoomCommand message,
            CancellationToken token)
        {
            var tutor = await _userRepository.LoadAsync(message.TutorId, token);
            if (tutor.Tutor?.State != ItemState.Ok)
            {
                throw new InvalidOperationException("user is not a tutor");
            }

            var student = await _userRepository.LoadAsync(message.StudentId, token);

            var chatRoomIdentifier = ChatRoom.BuildChatRoomIdentifier(new[] { tutor.Id, student.Id });
            var url = await _googleDocument.CreateOnlineDocAsync(chatRoomIdentifier, token);
            tutor.AddFollower(student);
            var studyRoom = new StudyRoom(tutor.Tutor, student, url);
            await _studyRoomRepository.AddAsync(studyRoom, token);
            return new CreateStudyRoomCommandResult(studyRoom.Id);
        }
    }
}