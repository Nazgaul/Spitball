using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class RequestTutorCommandHandler : ICommandHandler<RequestTutorCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IChatRoomRepository _chatRoomRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<ChatMessage> _chatMessageRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<Lead> _leadRepository;

        public RequestTutorCommandHandler(ITutorRepository tutorRepository, IChatRoomRepository chatRoomRepository, IRegularUserRepository userRepository, IRepository<ChatMessage> chatMessageRepository, IRepository<Course> courseRepository, IRepository<University> universityRepository, IRepository<Lead> leadRepository)
        {
            _tutorRepository = tutorRepository;
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
            _courseRepository = courseRepository;
            _universityRepository = universityRepository;
            _leadRepository = leadRepository;
        }

        public async Task ExecuteAsync(RequestTutorCommand message, CancellationToken token)
        {
            var needToRegisterLead = true;
            if (message.UserId.HasValue)
            {
                var usersIds = await _tutorRepository.GetTutorsByCourseAsync(message.Course, message.UserId.Value, token);
                var user = await _userRepository.LoadAsync(message.UserId.Value, token);
                foreach (var userId in usersIds)
                {
                    needToRegisterLead = false;
                    var users = new[] { userId, message.UserId.Value };
                    var chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, token);

                    var chatMessage = new ChatTextMessage(user, message.ChatText, chatRoom);
                    chatRoom.AddMessage(chatMessage);
                    await _chatRoomRepository.UpdateAsync(chatRoom, token);
                    await _chatMessageRepository.AddAsync(chatMessage, token);

                }


            }

            if (needToRegisterLead)
            {
                var course = await _courseRepository.LoadAsync(message.Course, token);
                University university = null;
                if (message.UniversityId != null)
                {

                    university = await _universityRepository.LoadAsync(message.UniversityId, token);
                }

                User user = null;
                if (message.UserId.HasValue)
                {
                     user = await _userRepository.LoadAsync(message.UserId.Value, token);
                }
                var lead = new Lead(course, message.LeadText, university,message.Referer, user,message.Name,message.PhoneNumber,message.Email);
                await _leadRepository.AddAsync(lead, token);
            }
        }
    }
}