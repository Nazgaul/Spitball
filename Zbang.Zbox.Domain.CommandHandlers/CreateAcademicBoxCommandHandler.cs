using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateAcademicBoxCommandHandler : CreateBoxCommandHandler
    {
        private readonly IAcademicBoxRepository m_AcademicRepository;
        private readonly IRepository<Library> m_LibraryRepository;
        private readonly IAcademicBoxThumbnailProvider m_AcademicBoxThumbnailProvider;
        private readonly IBlobProvider m_BlobProvider;

        public CreateAcademicBoxCommandHandler(
            IBoxRepository boxRepository,
            IUserRepository userRepository,
            IRepository<UserBoxRel> userBoxRelRepository,
            IRepository<Library> libraryRepository,
            IAcademicBoxRepository academicRepository,
            IAcademicBoxThumbnailProvider academicBoxThumbnailProvider,
            IBlobProvider blobProvider
            )
            : base(boxRepository, userRepository, userBoxRelRepository)
        {
            m_AcademicRepository = academicRepository;
            m_LibraryRepository = libraryRepository;
            m_AcademicBoxThumbnailProvider = academicBoxThumbnailProvider;
            m_BlobProvider = blobProvider;
        }
        public override CreateBoxCommandResult Execute(CreateBoxCommand command)
        {
            var academicCommand = command as CreateAcademicBoxCommand;
            if (academicCommand == null)
            {
                throw new InvalidCastException("can't cast CreateBox to CreateAcademicBox");
            }
            if (command.BoxName.Length > Box.NameLength)
            {
                throw new OverflowException("Box Name exceed" + Box.NameLength);
            }

            User user = UserRepository.Get(command.UserId);
            ValidateUser(user);

            Library library = m_LibraryRepository.Get(academicCommand.NodeId);

            if (library == null)
            {
                throw new NullReferenceException("library");
            }

            if (library.AmountOfNodes > 0)
            {
                throw new ArgumentException("cannot add box to library with nodes");
            }
            var universityUser = user.University.DataUnversity ?? user.University;
            if (!Equals(universityUser, library.University))
            {
                throw new ArgumentException("library user is not user university");
            }


            var box = m_AcademicRepository.CheckIfExists(academicCommand.CourseCode, universityUser, academicCommand.Professor
                , academicCommand.BoxName);
            if (box != null)
            {
                throw new BoxNameAlreadyExistsException();
            }
            var picturePath = m_AcademicBoxThumbnailProvider.GetAcademicBoxThumbnail();
            box = new AcademicBox(academicCommand.BoxName, universityUser,
                  academicCommand.CourseCode, academicCommand.Professor, library, 
                  picturePath, user, m_BlobProvider.GetThumbnailUrl(picturePath));

            m_LibraryRepository.Save(library);
            box.UserBoxRelationship.Add(new UserBoxRel(universityUser, box, UserRelationshipType.Owner));
            if (universityUser.Id != user.Id)
            {
                box.UserBoxRelationship.Add(new UserBoxRel(user, box, UserRelationshipType.Subscribe));
                SaveRepositories(user, box);

            }
            SaveRepositories(universityUser, box);
            box.CalculateMembers();
            m_AcademicRepository.Save(box,true);
            box.GenerateUrl();
            m_AcademicRepository.Save(box);
            var result = new CreateBoxCommandResult(box, universityUser.UniversityName);

            return result;
        }


        private void ValidateUser(User user)
        {
            if (user == null) throw new ArgumentNullException("user");
            if (user.University == null)
            {
                throw new ArgumentException("User need to connect to a university first");
            }
        }
    }
}
