using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateStudyRoomCommandHandler : ICommandHandler<CreateStudyRoomCommand, CreateStudyRoomCommandResult>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IGoogleDocument _googleDocument;
        private readonly IChatRoomRepository _chatRoomRepository;

        public CreateStudyRoomCommandHandler(IRegularUserRepository userRepository,
            IRepository<StudyRoom> studyRoomRepository, IGoogleDocument googleDocument, IChatRoomRepository chatRoomRepository)
        {
            _userRepository = userRepository;
            _studyRoomRepository = studyRoomRepository;
            _googleDocument = googleDocument;
            _chatRoomRepository = chatRoomRepository;
        }

        public async Task<CreateStudyRoomCommandResult> ExecuteAsync(CreateStudyRoomCommand message,
            CancellationToken token)
        {
            var tutor = await _userRepository.LoadAsync(message.TutorId, token);
            if (tutor.Tutor?.State != ItemState.Ok)
            {
                throw new InvalidOperationException($"user is not a tutor {message.TutorId}");
            }

            var students = message.StudentsId.Select(s => _userRepository.Load(s)).ToList();
            var usersId = message.StudentsId.Union(new[] { tutor.Id }).ToList();

            if (usersId.Count > 1)
            {
                var chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(usersId, token);
                chatRoom.AddTextMessage(tutor, message.TextMessage);
                tutor.AddFollowers(students);
            }

            var documentName = $"{message.Name}-{Guid.NewGuid()}";
            var googleDocUrl = await _googleDocument.CreateOnlineDocAsync(documentName, token);


            StudyRoom studyRoom;
            if (message.Type == StudyRoomType.Broadcast)
            {
                studyRoom = new BroadCastStudyRoom(tutor.Tutor, students, googleDocUrl,
                   message.Name, message.Price, message.BroadcastTime!.Value, message.Description);
                await _studyRoomRepository.AddAsync(studyRoom, token);
            }
            else
            {
                studyRoom = new PrivateStudyRoom(tutor.Tutor, students, googleDocUrl,
                   message.Name, message.Price);
                await _studyRoomRepository.AddAsync(studyRoom, token);
            }

            return new CreateStudyRoomCommandResult(studyRoom.Id, studyRoom.Identifier);
        }
    }
}