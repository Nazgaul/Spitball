using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Repositories;
using Rhino.Mocks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class DeleteQuestionCommandHandlerTests
    {
        private IRepository<Comment> m_StubQuestionRepository;
        private IRepository<Reputation> m_StubReputationRepository;
        private IBoxRepository m_StubBoxRepository;
        private IUserRepository m_UserRepository;

        [TestInitialize]
        public void Setup()
        {
            m_StubQuestionRepository = MockRepository.GenerateStub<IRepository<Comment>>();
            m_StubReputationRepository = MockRepository.GenerateStub<IRepository<Reputation>>();
            m_StubBoxRepository = MockRepository.GenerateStub<IBoxRepository>();
            m_UserRepository = MockRepository.GenerateStub<IUserRepository>();


        }

        [TestMethod]
        public void Handle_DeleteQuestionWithPermission_DeleteQuestion()
        {
            var questionId = Guid.Parse("24b2f722-6a8e-4d27-89b2-a2e2008b2aa3");
            var userId = 9470L;
            var command = new DeleteCommentCommand(questionId, userId);

            var user = new User("some email", " some small image", "some largeImage", "some first name", "some middle name", "some last name", true, false);
            user.GetType().GetProperty("Id").SetValue(user, userId);
            var ownerBox = new User("some email1", " some small image1", "some largeImage1", "some first name1", "some middle name1", "some last name1", true, false);
            var box = new Box("some box", ownerBox, Infrastructure.Enums.BoxPrivacySettings.MembersOnly);
            var question = new Comment(user, "some text", box, questionId, null);

            m_StubQuestionRepository.Stub(x => x.Load(questionId)).Return(question);

            var commandHanlder = new DeleteCommentCommandHandler(m_StubQuestionRepository, m_StubBoxRepository, m_StubReputationRepository, null, m_UserRepository);

            commandHanlder.Handle(command);
        }
    }
}
