//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Command.Command;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;

//namespace Cloudents.Command.CommandHandler
//{
//    public class AddUserTagCommandHandler : ICommandHandler<AddUserTagCommand>
//    {
//        private readonly ITagRepository _tagRepository;
//        private readonly IRepository<RegularUser> _userRepository;

//        public AddUserTagCommandHandler(ITagRepository tagRepository, IRepository<RegularUser> userRepository)
//        {
//            _tagRepository = tagRepository;
//            _userRepository = userRepository;
//        }

//        public async Task ExecuteAsync(AddUserTagCommand message, CancellationToken token)
//        {
//            var user = await _userRepository.LoadAsync(message.UserId, token);
//            var tag = await _tagRepository.GetOrAddAsync(message.Tag, token);
//            if (user.Tags.Add(tag))
//            {
//                tag.Count++;
//                await _tagRepository.UpdateAsync(tag, token);
//            }
//            await _userRepository.UpdateAsync(user, token);
//        }
//    }
//}