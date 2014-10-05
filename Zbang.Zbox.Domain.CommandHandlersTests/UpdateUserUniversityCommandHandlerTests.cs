using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.DataAccess;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class UpdateUserUniversityCommandHandlerTests
    {
        private IUserRepository m_StubUserRepository;
        private IRepository<University> m_StubUniversityRepository;
        private IRepository<RussianDepartment> m_StubDepartmentRepository;
        private IRepository<Student> m_StubStudentRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
            m_StubUniversityRepository = MockRepository.GenerateStub<IRepository<University>>();

            m_StubDepartmentRepository = MockRepository.GenerateStub<IRepository<RussianDepartment>>();
            m_StubStudentRepository = MockRepository.GenerateStub<IRepository<Student>>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_UserChooseUniversityWithCodeAndDontHaveCode_ThrowArgumentException()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, 0, null, null, null, null);


            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false, "en-US");
            var someUniversity = new University(someUniversityId, "some name", "some country", "some l img", "some email");

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository, m_StubDepartmentRepository, m_StubStudentRepository);
            commandHanlder.Handle(command);

        }

        [TestMethod]
        public void Handle_UserChooseUniversityWithCOdeAndHaveCode_OK()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, null, "N10028", null, null, null);


            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false, "en-US");
            var someUniversity = new University(someUniversityId, "some name", "some country", "some l img", "some email");

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository, m_StubDepartmentRepository, m_StubStudentRepository);
            commandHanlder.Handle(command);

            m_StubUserRepository.AssertWasCalled(x => x.Save(someUser));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Handle_UserChooseUniversityWithCodeAndHaveInvalidCode_ThrowArgumentException()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, null, "N12345", null, null, null);


            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false, "en-US");
            var someUniversity = new University(someUniversityId, "some name", "some country",  "some l img", "some email");

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository, m_StubDepartmentRepository, m_StubStudentRepository);
            commandHanlder.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Handle_UserChooseUniversityWithCodeAndAddUsedCode_ThrowException()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, null, "N12345", null, null, null);


            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false, "en-US");
            var someOtherUser = new User("some email2", " some small image2", "some largeImage2", "some first name2", "some middle name2", "some last name2", true, false, "en-US");
            var someUniversity = new University(someUniversityId, "some name", "some country",  "some l img", "some email");

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);


            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUserRepository.Stub(x => x.IsNotUsedCode("N10028", 3L)).Return(true);

            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository, m_StubDepartmentRepository, m_StubStudentRepository);
            commandHanlder.Handle(command);

        }

        [TestMethod]
        public void Handle_UserChooseUniversityWithCodeAndAlreadyHaveThatCode_Ok()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, 0, null, null, null, null);


            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false, "en-US");
            var someUniversity = new University(someUniversityId, "some name", "some country",  "some l img", "some email");

            someUser.Code = "N10028";
            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);


            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository, m_StubDepartmentRepository, m_StubStudentRepository);
            commandHanlder.Handle(command);

            m_StubUserRepository.AssertWasCalled(x => x.Save(someUser));
        }

        [TestMethod]
        public void Handle_UserChooseUniversityWithoutCode_Ok()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, 0, null, null, null, null);


            var someUser = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false, "en-US");
            var someUniversity = new University(someUniversityId,"some name","some country", "some l img", "some email");

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository, m_StubDepartmentRepository, m_StubStudentRepository);
            commandHanlder.Handle(command);

            m_StubUserRepository.AssertWasCalled(x => x.Save(someUser));
        }
    }

}
