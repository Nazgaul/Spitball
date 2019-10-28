using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ILeadRepository _leadRepository;

        public RequestTutorCommandHandler(ITutorRepository tutorRepository,
            IChatRoomRepository chatRoomRepository,
            IRegularUserRepository userRepository,
            IRepository<ChatMessage> chatMessageRepository,
            IRepository<Course> courseRepository,
            ILeadRepository leadRepository)
        {
            _tutorRepository = tutorRepository;
            _chatRoomRepository = chatRoomRepository;
            _userRepository = userRepository;
            _chatMessageRepository = chatMessageRepository;
            _courseRepository = courseRepository;
            _leadRepository = leadRepository;
        }

        public async Task ExecuteAsync(RequestTutorCommand message, CancellationToken token)
        {

            Tutor tutor = null;
            if (message.TutorId.HasValue)
            {
                if (message.UserId == message.TutorId.Value)
                {
                    throw new ArgumentException("You cannot request tutor to yourself");
                }
                tutor = await _tutorRepository.LoadAsync(message.TutorId.Value, token);
            }

            var course = await _courseRepository.LoadAsync(message.Course, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);

            var lead = new Lead(course, message.LeadText,
                 message.Referer, user,
                tutor, message.UtmSource);
            await _leadRepository.AddAsync(lead, token);

            var tutorsIds = new List<long>();
            if (message.MoreTutors)
            {
                var needToSendToMoreTutors = await _leadRepository.NeedToSendMoreTutorsAsync(message.UserId, token);
                if (needToSendToMoreTutors)
                {
                    var t = await _tutorRepository.GetTutorsByCourseAsync(message.Course, message.UserId, user.Country,
                        token);
                    tutorsIds.AddRange(t);
                }
            }

            if (tutor != null)
            {
                tutorsIds.Add(tutor.Id);
            }

            foreach (var userId in tutorsIds.Distinct())
            {
                var users = new[] { userId, message.UserId };
                var chatRoom = await _chatRoomRepository.GetOrAddChatRoomAsync(users, token);
                if (chatRoom.Extra == null)
                {
                    chatRoom.Extra = new ChatRoomAdmin(chatRoom);
                }
                chatRoom.Extra.Lead = lead;

                await _chatRoomRepository.UpdateAsync(chatRoom, token);
                var chatMessage = new ChatTextMessage(user, message.ChatText, chatRoom);
                chatRoom.AddMessage(chatMessage);
                await _chatRoomRepository.UpdateAsync(chatRoom, token);
                await _chatMessageRepository.AddAsync(chatMessage, token);
            }

        }
    }
}