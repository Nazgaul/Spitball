//using Autofac.Extras.Moq;
//using Cloudents.Command.Command;
//using Cloudents.Command.CommandHandler;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Enum;
//using Cloudents.Core.Interfaces;
//using Moq;
//using System;
//using System.Globalization;
//using System.Threading.Tasks;
//using Xunit;

//namespace Cloudents.Core.Test.CommandHandler
//{
//    public class CreateAnswerCommandHandlerTests
//    {

//        public static string ClassName = "Some Class Name";
//        [Fact]
//        public async Task ExecuteAsync_QuestionWithAlreadyUserAnswer_Error()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                long questionId = 1, userId = 1;
//                var stubCourse = new Mock<Course>();
//                var stubUniversity = new Mock<University>();
//                var user = new RegularUser("some Email", "some name", Language.English);
//                var questionUser = new RegularUser("other email", "other name",
//                    Language.English);
//               var question = new Question(
//                                new QuestionSubject(), "some text very very big test",
//                                0, 0, questionUser,

//                                CultureInfo.InvariantCulture, stubCourse.Object, stubUniversity.Object);

//                question.MakePublic();
//                question.AddAnswer("some textxxxxxxxxxxx", 0, user, CultureInfo.InvariantCulture);



//                mock.Mock<IRepository<RegularUser>>().Setup(s => s.LoadAsync(userId, default)).ReturnsAsync(user);
//                mock.Mock<IRepository<Question>>().Setup(s => s.GetAsync(questionId, default)).ReturnsAsync(question);
//                var instance = mock.Create<CreateAnswerCommandHandler>();

//                var command = new CreateAnswerCommand(questionId, "someText", userId, null);

//                await Assert.ThrowsAsync<InvalidOperationException>(() => instance.ExecuteAsync(command, default));
//            }
//        }

//        //[Fact]
//        //public async Task ExecuteAsync_DefaultProcess_Ok()
//        //{
//        //    using (var mock = AutoMock.GetLoose())
//        //    {
//        //        long questionId = 1, userId = 1;
//        //      //  var user = new RegularUser("some Email", "some name", Language.English);
//        //        var questionUser = new RegularUser("other email", "other name",
//        //            Language.English);


//        //        var stubCourse = new Mock<Course>();
//        //        var stubUniversity = new Mock<University>();
//        //        var question = new Question(new QuestionSubject(), "some text", 0, 0, questionUser,
//        //    CultureInfo.InvariantCulture, stubCourse.Object, stubUniversity.Object);

//        //        var stubUser = new Mock<RegularUser>();
//        //        stubUser.Setup(s => s.Id).Returns(userId);
//        //        stubUser.Setup(s => s.Transactions).Returns(new UserTransactions());

//        //        var stubQuestionUser = new Mock<RegularUser>();

//        //        var stubQuestion = new Mock<Question>();

//        //        stubQuestion.Setup(s => s.Id).Returns(questionId);
//        //        stubQuestion.Setup(s => s.Status).Returns(ItemStatus.Public);
//        //        stubQuestion.Setup(s => s.User).Returns(stubQuestionUser.Object);
//        //        stubQuestion.Setup(s => s.Answers).Returns(new List<Answer>());

//        //        mock.Mock<IRepository<RegularUser>>()
//        //            .Setup(s => s.LoadAsync(userId, default))
//        //            .ReturnsAsync(stubUser.Object);
//        //        mock.Mock<IRepository<Question>>()
//        //            .Setup(s => s.GetAsync(questionId, default))
//        //            .ReturnsAsync(stubQuestion.Object);
//        //        var instance = mock.Create<CreateAnswerCommandHandler>();
//        //        var command = new CreateAnswerCommand(questionId, "someText", userId, null);
//        //        await instance.ExecuteAsync(command, default);
//        //    }
//        //}
//    }
//}