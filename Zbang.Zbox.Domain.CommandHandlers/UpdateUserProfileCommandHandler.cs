using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Profile;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IUniversityRepository m_UniversityRepository;
        private readonly IQueueProvider m_QueueProvider;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository, IUniversityRepository universityRepository,
            IQueueProvider queueProvider)
        {
            m_UserRepository = userRepository;
            m_UniversityRepository = universityRepository;
            m_QueueProvider = queueProvider;
        }
        public void Handle(UpdateUserProfileCommand command)
        {
            User user = m_UserRepository.Get(command.Id);
            if (user == null)
            {
                throw new Infrastructure.Exceptions.UserNotFoundException("user doesn't exists");
            }
            //University university = GetUniversity(command.University);

            user.UpdateUserProfile(command.UserName, command.PicturePath, command.LargePicturePath);

            //if (university != null)
            //{
            //    m_QueueProvider.InsertMessageToTranaction(new Infrastructure.Transport.AddAFriendData(user.Id, university.Id));
            //    //user.AddAFriend(university);
            //    if (university.DataUnversity == null)
            //    {
            //        command.UniversityId = university.Id;
            //    }
            //    else
            //    {
            //        command.UniversityWrapperId = university.Id;
            //        command.UniversityId = university.DataUnversity.Id;
            //    }
            //}
            m_UserRepository.Save(user);
        }

        private University GetUniversity(string universityName)
        {
            if (string.IsNullOrEmpty(universityName))
            {
                return null;
            }
            var university = m_UniversityRepository.GetUniversity(universityName);
            return university;
        }
    }
}
