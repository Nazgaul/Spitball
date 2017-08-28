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
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<University> m_UniversityRepository;
        private readonly IRepository<Student> m_StudentRepository;

        public UpdateUserUniversityCommandHandler(IUserRepository userRepository,
            IRepository<University> universityRepository,
            IRepository<Student> studentRepository
            )
        {
            m_UserRepository = userRepository;
            m_UniversityRepository = universityRepository;
            m_StudentRepository = studentRepository;
        }
        public void Handle(UpdateUserUniversityCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var university = m_UniversityRepository.Load(message.UniversityId);
            var user = m_UserRepository.Load(message.UserId);
            var studentId = message.StudentId ?? user.StudentId;

            // ReSharper disable once PossibleUnintendedReferenceComparison NHibernate doesn't support equals
            var studentsIdsInUniversity = m_StudentRepository.Query().Where(w => w.University == university);
            bool needId = studentsIdsInUniversity.Any();
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

            m_UserRepository.Save(user);
        }

        //private bool ValidateUserCode(string p)
        //{
        //    if (string.IsNullOrEmpty(p))
        //    {
        //        throw new ArgumentNullException("p");
        //    }
        //    if (!Char.IsLetter(p, 0))
        //    {
        //        throw new ArgumentException(@"need first to be letter", "p");
        //    }
        //    var notHavingDigits = System.Text.RegularExpressions.Regex.IsMatch(p, @"^[^\d]*$");
        //    if (notHavingDigits)
        //    {
        //        throw new NullReferenceException("code need to have digits");
        //    }
        //    int sumOfDigits = p.Where(e => e >= '0' && e <= '9')
        //                    .Reverse()
        //                    .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
        //                    .Sum(e => e / 10 + e % 10);

        //    //int sumOfDigits = creditCardNumber
        //    //               .Reverse()
        //    //               .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
        //    //               .Sum((e) => e / 10 + e % 10);

        //    //// If the final sum is divisible by 10, then the credit card number
        //    //   is valid. If it is not divisible by 10, the number is invalid.            
        //    var isValid = sumOfDigits % 13 == 0;
        //    if (isValid)
        //    {
        //        //  return m_UserRepository.IsNotUsedCode(p, userId);
        //    }
        //    return isValid;
        //}

    }
}
