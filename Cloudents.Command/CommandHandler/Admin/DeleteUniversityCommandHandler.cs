//using Cloudents.Command.Command.Admin;
//using Cloudents.Core.Interfaces;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;

//namespace Cloudents.Command.CommandHandler.Admin
//{
//    public class DeleteUniversityCommandHandler : ICommandHandler<DeleteUniversityCommand>
//    {
//        private readonly IRepository<University> _universityRepository;

//        public DeleteUniversityCommandHandler(IRepository<University> universityRepository)
//        {
//            _universityRepository = universityRepository;
//        }

//        public async Task ExecuteAsync(DeleteUniversityCommand message, CancellationToken token)
//        {
//            var universityToRemove = await _universityRepository.LoadAsync(message.Id, token);
//            await _universityRepository.DeleteAsync(universityToRemove, token);
//        }
//    }
//}
