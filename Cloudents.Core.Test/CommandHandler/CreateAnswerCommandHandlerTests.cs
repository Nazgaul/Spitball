using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Cloudents.Core.Command;
using Cloudents.Core.CommandHandler;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Core.Test.CommandHandler
{
    [TestClass]
    public class CreateAnswerCommandHandlerTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task ExecuteAsync_QuestionWithAlreadyUserAnswer_Error()
        {
            long questionId = 1, userId = 1;
            var user = new User("some Email", "some name", "some private key");
            var questionUser = new User("other email", "other name", "other private key")
            {
                Id = 2
            };
            var question = new Question(new QuestionSubject(), "some text", 0, 0, questionUser);

            question.Answers.Add(new Answer(question, "some text", 0, user));

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRepository<User>>().Setup(s => s.LoadAsync(userId, default)).ReturnsAsync(user);
                mock.Mock<IRepository<Question>>().Setup(s => s.LoadAsync(questionId, default)).ReturnsAsync(question);
                var instance = mock.Create<CreateAnswerCommandHandler>();

                var command = new CreateAnswerCommand(questionId, "someText", userId, null, null);


                await instance.ExecuteAsync(command, default);
            }
        }
    }
}