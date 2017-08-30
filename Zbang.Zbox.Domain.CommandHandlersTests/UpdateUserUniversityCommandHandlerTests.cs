using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.DataAccess;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class UpdateUserUniversityCommandHandlerTests
    {
        private IUserRepository m_StubUserRepository;
        private IRepository<University> m_StubUniversityRepository;
        private IRepository<Student> m_StubStudentRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
            m_StubUniversityRepository = MockRepository.GenerateStub<IRepository<University>>();

            m_StubStudentRepository = MockRepository.GenerateStub<IRepository<Student>>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_UserChooseUniversityWithCodeAndDontHaveCode_ThrowArgumentException()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, null);


            var someUser = new User("some email",  "some largeImage", "some first name", "some last name", "en-US",Sex.NotApplicable);
            var someUniversity = new University(someUniversityId, "some name", "some country", 0);

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHandler = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository, m_StubStudentRepository);
            commandHandler.Handle(command);

        }

       

        [TestMethod]
        public void Handle_UserChooseUniversityWithCodeAndAlreadyHaveThatCode_Ok()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, null);


            var someUser = new User("some email", "some largeImage", "some first name", "some last name", "en-US", Sex.NotApplicable);
            var someUniversity = new University(someUniversityId, "some name", "some country", 0);

            someUser.StudentId = "N10028";
            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);


            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHandler = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository,  m_StubStudentRepository);
            commandHandler.Handle(command);

            m_StubUserRepository.AssertWasCalled(x => x.Save(someUser));
        }

        [TestMethod]
        public void Handle_UserChooseUniversityWithoutCode_Ok()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, null);


            var someUser = new User("some email", "some largeImage", "some first name", "some last name", "en-US", Sex.NotApplicable);
            var someUniversity = new University(someUniversityId, "some name", "some country", 0);

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHandler = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository,  m_StubStudentRepository);
            commandHandler.Handle(command);

            m_StubUserRepository.AssertWasCalled(x => x.Save(someUser));
        }
    }

}
