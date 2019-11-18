//using System.Threading.Tasks;
//using Autofac.Extras.Moq;
//using Cloudents.Core.Command.Admin;
//using Cloudents.Core.CommandHandler.Admin;
//using Xunit;

//namespace Cloudents.Core.Test.CommandHandler.Admin
//{
//    public class UpdateUserBalanceCommandHandlerTests
//    {
//        //[Fact]
//        //public async Task ExecuteAsync_NullUserInput_Ok()
//        //{
//        //    var command = new UpdateUserBalanceCommand();
//        //    using (var mock = AutoMock.GetLoose())
//        //    {
//        //        var instance = mock.Create<UpdateUserBalanceCommandHandler>();
//        //        await instance.ExecuteAsync(command, default);
//        //    }
//        //}

//        //[TestMethod]
//        //public async Task ExecuteAsync_UserLockedOut_Ok()
//        //{

//        //    var command = new UpdateUserBalanceCommand(new [] { userId });
//        //    using (var mock = AutoMock.GetLoose())
//        //    {
//        //        mock.Mock<IUserRepository>().Setup(s => s.LoadAsync(userId, default)).ReturnsAsync(user);
//        //        var instance = mock.Create<UpdateUserBalanceCommandHandler>();
//        //        await instance.ExecuteAsync(command, default);
//        //    }
//        //}
//    }
//}