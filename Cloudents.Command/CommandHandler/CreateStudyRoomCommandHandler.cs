using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateStudyRoomCommandHandler : ICommandHandler<CreateStudyRoomCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IGoogleDocument _googleDocument;
        private readonly IChatRoomRepository _chatRoomRepository;

        public CreateStudyRoomCommandHandler(IRegularUserRepository userRepository, IRepository<StudyRoom> studyRoomRepository, IGoogleDocument googleDocument, IChatRoomRepository chatRoomRepository)
        {
            _userRepository = userRepository;
            _studyRoomRepository = studyRoomRepository;
            _googleDocument = googleDocument;
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task ExecuteAsync(CreateStudyRoomCommand message, CancellationToken token)
        {
            var userTutor = await _userRepository.LoadAsync(message.TutorId, token);
            if (userTutor.Tutor?.State != ItemState.Ok)
            {
                throw new InvalidOperationException("user is not a tutor");
            }

            var student = await _userRepository.LoadAsync(message.StudentId, token);

            var chatRoomIdentifier = ChatRoom.BuildChatRoomIdentifier(new[] { userTutor.Id, student.Id });
            var chatRoom = await _chatRoomRepository.GetChatRoomAsync(chatRoomIdentifier, token);
            if (chatRoom.Messages.Count == 0)
            {
                throw new InvalidOperationException("no active conversation");
            }

            var url = await _googleDocument.CreateOnlineDocAsync(chatRoomIdentifier, token);

            userTutor.AddFollower(student);

            var studyRoom = new StudyRoom(userTutor.Tutor, student, url);
            await _studyRoomRepository.AddAsync(studyRoom, token);


        }
    }
}