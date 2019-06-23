using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
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

        public RequestTutorCommandHandler(ITutorRepository tutorRepository,
            IChatRoomRepository chatRoomRepository,
            IRegularUserRepository userRepository,
            IRepository<ChatMessage> chatMessageRepository,
            IRepository<Course> courseRepository,
            IRepository<University> universityRepository,
            IRepository<Lead> leadRepository)
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
            // var needToRegisterLead = true;
            Tutor tutor = null;
            if (message.TutorId.HasValue)
            {
                tutor = await _tutorRepository.LoadAsync(message.TutorId.Value, token);
            }

            var course = await _courseRepository.LoadAsync(message.Course, token);
            University university = null;
            if (message.UniversityId != null)
            {

                university = await _universityRepository.LoadAsync(message.UniversityId, token);
            }

            User userLead = null;
            if (message.UserId.HasValue)
            {
                userLead = await _userRepository.LoadAsync(message.UserId.Value, token);
            }
            var lead = new Lead(course, message.LeadText,
                university, message.Referer, userLead,
                message.Name, message.PhoneNumber, message.Email, tutor, message.UtmSource);
            await _leadRepository.AddAsync(lead, token);




            if (message.UserId.HasValue)
            {
                var tutorsIds = await _tutorRepository.GetTutorsByCourseAsync(message.Course, message.UserId.Value, token);
                if (tutor != null)
                {
                    tutorsIds.Add(tutor.Id);

                }
                var user = await _userRepository.LoadAsync(message.UserId.Value, token);
                foreach (var userId in tutorsIds)
                {
                    //  needToRegisterLead = false;
                    var users = new[] { userId, message.UserId.Value };
                    var chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, token);

                    chatRoom.Extra = new ChatRoomAdmin(ChatRoomStatus.Default, lead);

                    var chatMessage = new ChatTextMessage(user, message.ChatText, chatRoom);
                    chatRoom.AddMessage(chatMessage);
                    await _chatRoomRepository.UpdateAsync(chatRoom, token);
                    await _chatMessageRepository.AddAsync(chatMessage, token);

                }
            }

            // if (needToRegisterLead)
            // {

            // }
        }
    }
}