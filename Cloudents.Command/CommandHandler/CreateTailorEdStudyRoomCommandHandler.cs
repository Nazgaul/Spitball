using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class CreateTailorEdStudyRoomCommandHandler : ICommandHandler<CreateTailorEdStudyRoomCommand>
    {
        private readonly IFictiveUserRepository _userRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IGoogleDocument _googleDocument;
        private readonly IShortIdGenerator _shortIdGenerator;

        public CreateTailorEdStudyRoomCommandHandler(IFictiveUserRepository userRepository,
            ITutorRepository tutorRepository, IRepository<StudyRoom> studyRoomRepository,
            IGoogleDocument googleDocument, IShortIdGenerator shortIdGenerator)
        {
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
            _studyRoomRepository = studyRoomRepository;
            _googleDocument = googleDocument;
            _shortIdGenerator = shortIdGenerator;
        }

        public async Task ExecuteAsync(CreateTailorEdStudyRoomCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.GetTailorEdTutorAsync(token);

            //var students = new List<SystemUser>();
            //for (int i = 0; i < message.AmountOfUsers; i++)
            //{
            //    var user = await _userRepository.GetRandomFictiveUserAsync(i, token);
            //    students.Add(user);
            //}
            var documentName = $"{Guid.NewGuid()}";
            var googleDocUrl = await _googleDocument.CreateOnlineDocAsync(documentName, token);

            var studentCode = _shortIdGenerator.GenerateShortId(8);
            StudyRoom studyRoom = new TailorEdStudyRoom(tutor, message.AmountOfUsers, studentCode, googleDocUrl);
            await _studyRoomRepository.AddAsync(studyRoom, token);
            message.StudyRoomId = studyRoom.Id;
            message.TutorId = tutor.Id;
            message.Code = studentCode;
        }
    }
}