using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserUniversityCommandHandler : ICommandHandler<UpdateUserUniversityCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<University> _universityRepository;
        private readonly IRepository<Student> _studentRepository;

        public UpdateUserUniversityCommandHandler(IUserRepository userRepository,
            IRepository<University> universityRepository,
            IRepository<Student> studentRepository
            )
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
            _studentRepository = studentRepository;
        }

        public void Handle(UpdateUserUniversityCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var university = _universityRepository.Load(message.UniversityId);
            var user = _userRepository.Load(message.UserId);
            var studentId = message.StudentId ?? user.StudentId;

            // ReSharper disable once PossibleUnintendedReferenceComparison NHibernate doesn't support equals
            var studentsIdsInUniversity = _studentRepository.Query().Where(w => w.University == university);
            var needId = studentsIdsInUniversity.Any();
            if (needId && string.IsNullOrEmpty(studentId))
            {
                throw new ArgumentException("need id for this university");
            }
            if (needId)
            {
                var student = studentsIdsInUniversity.FirstOrDefault(w => w.ID == studentId);

                if (student == null)
                {
                    throw new ArgumentException("need id for this university");
                }
                //userCode = student.ID;
            }
            message.UniversityId = university.Id;
            message.UniversityDataId = university.UniversityData?.Id ?? university.Id;
            user.UpdateUniversity(university, message.StudentId);

            _userRepository.Save(user);
        }
    }
}
