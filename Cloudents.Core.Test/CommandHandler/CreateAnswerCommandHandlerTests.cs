using Autofac.Extras.Moq;
using Cloudents.Command.Command;
using Cloudents.Command.CommandHandler;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Moq;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Core.Test.CommandHandler
{
    public class CreateAnswerCommandHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_QuestionWithAlreadyUserAnswer_Error()
        {
            long questionId = 1, userId = 1;
            var user = new RegularUser("some Email", "some name",  Language.English);
            var questionUser = new RegularUser("other email", "other name", 
                Language.English);
            //{
            //    Id = 2
            //};
            var question = new Question(
                new QuestionSubject(), "some text",
                0, 0, questionUser,
                
                CultureInfo.InvariantCulture,new Course("xxx"),new University("xxx","IL") );

            question.AddAnswer("some text", 0, user, CultureInfo.InvariantCulture);

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRepository<User>>().Setup(s => s.LoadAsync(userId, default)).ReturnsAsync(user);
                mock.Mock<IRepository<Question>>().Setup(s => s.GetAsync(questionId, default)).ReturnsAsync(question);
                var instance = mock.Create<CreateAnswerCommandHandler>();

                var command = new CreateAnswerCommand(questionId, "someText", userId, null);

                await Assert.ThrowsAsync<InvalidOperationException>(() => instance.ExecuteAsync(command, default));
            }
        }

        [Fact]
        public async Task ExecuteAsync_DefaultProcess_Ok()
        {
            long questionId = 1, userId = 1;
            var user = new RegularUser("some Email", "some name",  Language.English);
            var questionUser = new RegularUser("other email", "other name", 
                Language.English);
            //{
            //    Id = 2
            //};
            var question = new Question(new QuestionSubject(), "some text", 0, 0, questionUser,
                CultureInfo.InvariantCulture, new Course("xxx"), new University("xxx", "IL"));

            //question.Answers.Add(new Answer(question, "some text", 0, user));

            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IRepository<User>>().Setup(s => s.LoadAsync(userId, default)).ReturnsAsync(user);
                mock.Mock<IRepository<Question>>().Setup(s => s.GetAsync(questionId, default)).ReturnsAsync(question);
                var instance = mock.Create<CreateAnswerCommandHandler>();
                var command = new CreateAnswerCommand(questionId, "someText", userId, null);
                await instance.ExecuteAsync(command, default);
            }
        }
    }
}