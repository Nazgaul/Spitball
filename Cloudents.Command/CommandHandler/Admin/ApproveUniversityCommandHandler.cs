//using Cloudents.Command.Command.Admin;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Command.CommandHandler.Admin
//{
//    public class ApproveUniversityCommandHandler : ICommandHandler<ApproveUniversityCommand>
//    {
//        private readonly IRepository<University> _universityRepository;
//        public ApproveUniversityCommandHandler(IRepository<University> universityRepository)
//        {
//            _universityRepository = universityRepository;
//        }

//        public async Task ExecuteAsync(ApproveUniversityCommand message, CancellationToken token)
//        {
//            var university = await _universityRepository.LoadAsync(message.Id, token);
//            university.Approve();
//        }
//    }
//}
