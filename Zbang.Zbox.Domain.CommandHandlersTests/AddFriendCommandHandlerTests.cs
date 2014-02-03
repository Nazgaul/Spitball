using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;
using Rhino.Mocks.Constraints;

namespace Zbang.Zbox.Domain.CommandHandlersTests
{
    [TestClass]
    public class AddFriendCommandHandlerTests
    {
        //Name method to check_Schnario_ExpectedResult
        [TestMethod]
        public void Handle_AddAFriend_Saves()
        {
            IUserRepository userRepository = MockRepository.GenerateStub<IUserRepository>();
            
            var User = new User("some email", "some user", "some image", "some large image");

            var User2 = new User("some email", "some user", "some image", "some large image"); 
            var addFriendCommand = new AddFriendCommand(1,2);

            userRepository.Stub(rep => rep.Get(addFriendCommand.UserId)).Return(User);
            userRepository.Stub(rep => rep.Get(addFriendCommand.FriendId)).Return(User);

            AddFriendCommandHandler handler = new AddFriendCommandHandler(userRepository);
            handler.Handle(addFriendCommand);


            //userRepository.AssertWasCalled(rep => rep.Save(null), options => options.IgnoreArguments().Constraints(Is.Equal(User)));
            userRepository.AssertWasCalled(rep => rep.Save(User));
        }
    }
}
