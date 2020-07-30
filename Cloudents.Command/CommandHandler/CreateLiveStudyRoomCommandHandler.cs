//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Command.Command;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Exceptions;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Storage;

//namespace Cloudents.Command.CommandHandler
//{
//    public class CreateLiveStudyRoomCommandHandler : ICommandHandler<CreateLiveStudyRoomCommand>
//    {
//        private readonly ITutorRepository _tutorRepository;
//        private readonly IRepository<BroadCastStudyRoom> _studyRoomRepository;
//        private readonly IGoogleDocument _googleDocument;
//        private readonly ICronService _cronService;
//        private readonly IStudyRoomBlobProvider _blobProvider;

//        public CreateLiveStudyRoomCommandHandler(
//            IRepository<BroadCastStudyRoom> studyRoomRepository, IGoogleDocument googleDocument,
//            ITutorRepository tutorRepository, ICronService cronService, IStudyRoomBlobProvider blobProvider)
//        {
//            _studyRoomRepository = studyRoomRepository;
//            _googleDocument = googleDocument;
//            _tutorRepository = tutorRepository;
//            _cronService = cronService;
//            _blobProvider = blobProvider;
//        }

//        public async Task ExecuteAsync(CreateLiveStudyRoomCommand message,
//            CancellationToken token)
//        {
//            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
//            var documentName = $"{message.Name}-{Guid.NewGuid()}";
//            var googleDocUrl = await _googleDocument.CreateOnlineDocAsync(documentName, token);

//            StudyRoomSchedule? schedule = null;
//            if (message.Repeat.HasValue)
//            {
//                var endDate = message.EndDate?.AddDays(1);
//                var z = message.Repeat.Value switch
//                {
//                    StudyRoomRepeat.Daily => _cronService.BuildCronDaily(message.BroadcastTime),
//                    StudyRoomRepeat.Weekly => _cronService.BuildCronWeekly(message.BroadcastTime),
//                    StudyRoomRepeat.Custom => _cronService.BuildCronCustom(message.BroadcastTime, message.RepeatOn),
//                    _ => null
//                };
//                if (z == null)
//                {
//                    throw new ArgumentException("Not valid repeat");
//                }
//                if (message.EndAfterOccurrences.HasValue)
//                {
//                    if (message.EndAfterOccurrences.Value == 1)
//                    {
//                        endDate = message.BroadcastTime;
//                    }
//                    else
//                    {
//                        endDate =
//                            _cronService.CalculateEndTime(message.BroadcastTime, z, message.EndAfterOccurrences.Value - 1);
//                    }
//                }

//                if (!endDate.HasValue)
//                {
//                    throw new ArgumentException("end date is null");
//                }
//                schedule = new StudyRoomSchedule(z, endDate.Value, message.BroadcastTime);
//            }

//            var course = tutor.Courses.FirstOrDefault(f => f.Name == message.Name);
//            if (course != null)
//            {
//                throw new DuplicateRowException();
//            }
//            course = new Course(message.Name, tutor)
//            {
//                Description = message.Description,
//                Price = new Money(message.Price, tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol)
//            };
//            tutor.AddCourse(course);
//            //To persist the course if needed
//            //await _tutorRepository.UpdateAsync(tutor, default);
//            var studyRoom = new BroadCastStudyRoom(tutor, googleDocUrl,
//                course, message.Price,
//                message.BroadcastTime, message.Description, schedule);
//            await _studyRoomRepository.AddAsync(studyRoom, token);
//            await _courseRepository.AddAsync(course, token);

//            if (message.Image != null)
//            {
//                await _blobProvider.MoveAsync(message.Image, course.Id.ToString(), "0.jpg", token);
//            }

//            message.StudyRoomId = studyRoom.Id;
//            message.Identifier = studyRoom.Identifier;
//        }
//    }
//}