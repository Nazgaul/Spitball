//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;

//namespace Cloudents.Command.CommandHandler
//{
//    public class AddUserToChatCommandHandler : ICommandHandler<AddUserToChatCommand>
//    {
//        private readonly IRepository<StudyRoom> _studyRoomRepository;
//        private readonly IRegularUserRepository _userRepository;

//        public AddUserToChatCommandHandler(IRepository<StudyRoom> studyRoomRepository, IRegularUserRepository userRepository)
//        {
//            _studyRoomRepository = studyRoomRepository;
//            _userRepository = userRepository;
//        }


//        public async Task ExecuteAsync(AddUserToChatCommand message, CancellationToken token)
//        {
//            var studyRoom = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
//            var user = await _userRepository.LoadAsync(message.UserId, token);
//            if (studyRoom.ChatRoom == null)
//            {
//                return;
//            }

//            studyRoom.ChatRoom.AddUserToChat(user);
//        }
//    }
//}