using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.DataAccess;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class UpdateUserUniversityCommandHandlerTests
    {
        private IUserRepository m_StubUserRepository;
        private IUniversityRepository m_StubUniversityRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubUserRepository = MockRepository.GenerateStub<IUserRepository>();
            m_StubUniversityRepository = MockRepository.GenerateStub<IUniversityRepository>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Handle_UserChooseUniversityWithCodeAndDontHaveCode_ThrowArgumentException()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId);


            var someUser = new User("some email", "some user", "some img", "some l img");
            var someUniversity = new University("some university", "some img", "some l img", "test");

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository);
            commandHanlder.Handle(command);

        }

        [TestMethod]
        public void Handle_UserChooseUniversityWithCOdeAndHaveCode_OK()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, "N10028");


            var someUser = new User("some email", "some user", "some img", "some l img");
            var someUniversity = new University("some university", "some img", "some l img", "test");

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository);
            commandHanlder.Handle(command);

            m_StubUserRepository.AssertWasCalled(x => x.Save(someUser));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Handle_UserChooseUniversityWithCodeAndHaveInvalidCode_ThrowArgumentException()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, "N12345");


            var someUser = new User("some email", "some user", "some img", "some l img");
            var someUniversity = new University("some university", "some img", "some l img", "test");

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository);
            commandHanlder.Handle(command);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Handle_UserChooseUniversityWithCodeAndAddUsedCode_ThrowException()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId, "N12345");


            var someUser = new User("some email", "some user", "some img", "some l img");
            var someOtherUser = new User("some email2", "some user2", "some img2", "some l img2");
            var someUniversity = new University("some university", "some img", "some l img", "test");

            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);


            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUserRepository.Stub(x => x.IsNotUsedCode("N10028", 3L)).Return(true);

            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository);
            commandHanlder.Handle(command);

        }

        [TestMethod]
        public void Handle_UserChooseUniversityWithCodeAndAlreadyHaveThatCode_Ok()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId);


            var someUser = new User("some email", "some user", "some img", "some l img");
            var someUniversity = new University("some university", "some img", "some l img", "test");

            someUser.Code = "N10028";
            someUniversity.GetType().GetProperty("NeedCode").SetValue(someUniversity, true);


            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository);
            commandHanlder.Handle(command);

            m_StubUserRepository.AssertWasCalled(x => x.Save(someUser));
        }

        [TestMethod]
        public void Handle_UserChooseUniversityWithoutCode_Ok()
        {
            var someUniversityId = 1L;
            var someUserId = 2L;
            var command = new UpdateUserUniversityCommand(someUniversityId, someUserId);


            var someUser = new User("some email", "some user", "some img", "some l img");
            var someUniversity = new University("some university", "some img", "some l img", "test");

            m_StubUserRepository.Stub(x => x.Get(someUserId)).Return(someUser);
            m_StubUniversityRepository.Stub(x => x.Get(someUniversityId)).Return(someUniversity);

            var commandHanlder = new UpdateUserUniversityCommandHandler(m_StubUserRepository, m_StubUniversityRepository);
            commandHanlder.Handle(command);

            m_StubUserRepository.AssertWasCalled(x => x.Save(someUser));
        }
    }

}
