//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Event;
//using Cloudents.Core.Interfaces;
//using Cloudents.Core.Message.Email;
//using Cloudents.Core.Storage;

//namespace Cloudents.Core.EventHandler
//{
//    public class EmailStudyRoomVideoUploadEventHandler: IEventHandler<StudyRoomSessionVideoUploadedEvent>
//    {
//        private readonly IQueueProvider _queueProvider;
//        private readonly IStudyRoomSessionBlobProvider _blobProvider;
//            public EmailStudyRoomVideoUploadEventHandler(IQueueProvider queueProvider, IStudyRoomSessionBlobProvider blobProvider)
//            {
//                _queueProvider = queueProvider;
//                _blobProvider = blobProvider;
//            }

//        public async Task HandleAsync(StudyRoomSessionVideoUploadedEvent eventMessage, CancellationToken token)
//        {
//            var studyRoomSession = eventMessage.StudyRoomSession;
//            var tutor = studyRoomSession.StudyRoom.Tutor;

//            var users = studyRoomSession.StudyRoom.Users.Where(w => w.User.Id != tutor.Id);

//            foreach (var user in users)
//            {
//                var message = new StudyRoomVideoMessage(
//                    tutor.User.FirstName,
//                    user.User.Name,
//                    DateTime.UtcNow,
//                   // _blobProvider.DownloadVideoLink(studyRoomSession.StudyRoom.Id,studyRoomSession.SessionId),
//                    user.User.Email,
//                    user.User.Language
//                );

//               await _queueProvider.InsertMessageAsync(message,token);
//            }

            
//        }
//    }
//}