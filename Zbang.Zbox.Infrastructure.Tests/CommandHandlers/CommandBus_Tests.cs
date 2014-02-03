using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.CommandHandlers;
using Rhino.Mocks;
using Zbang.Zbox.Domain;

namespace Zbang.Zbox.Infrastructure.Tests.CommandHandlers
{
    [TestFixture]    
    public class CommandBus_Tests
    {
        [Test]
        public void Dispatch_CallWithRegisteredHandler_ExecuteAppropriateHandler()
        {
            //Arrange
            
            //Create Container
            UnityContainer container = new UnityContainer();

            //Create a mocked command handler
            ICommandHandler<CreateBoxCommand, CreateBoxCommandResult> createBoxCommandHandlerMock = MockRepository.GenerateStrictMock<ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>>();
            
            //Register Command Handler
            container.RegisterInstance<ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>>(createBoxCommandHandlerMock);

            //Create Bus
            ICommandBus bus = new CommandBus(container);

            //Create Command
            CreateBoxCommand command = new CreateBoxCommand(3, "Test", Enums.NotificationSettings.OnEveryChange, Enums.BoxPrivacySettings.Private);
            //CreateBoxCommand command = new CreateBoxCommand(1, "My Box", NotificationSettings.Off,BoxPrivacySettings.InvitationOnly);

            //Set expectation
            createBoxCommandHandlerMock.Expect(ch => ch.Execute(command)).Return(null);                
            
            //Act
            CreateBoxCommandResult commandResult = bus.Dispatch<CreateBoxCommand, CreateBoxCommandResult>(command);

            //Assert
            createBoxCommandHandlerMock.VerifyAllExpectations();
        }

        //[Test]
        //public void Dispatch_CallWithRegisteredHandler_GetCommandResult()
        //{
        //    //Arrange

        //    //Create Container
        //    UnityContainer container = new UnityContainer();

        //    //Create a mocked command handler
        //    ICommandHandler<CreateBoxCommand, CreateBoxCommandResult> createBoxCommandHandlerStub = MockRepository.GenerateStub<ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>>();
            
        //    //Register Command Handler
        //    container.RegisterInstance<ICommandHandler<CreateBoxCommand, CreateBoxCommandResult>>(createBoxCommandHandlerStub);

        //    //Create Bus
        //    ICommandBus bus = new CommandBus(container);

        //    //Create Command
        //    CreateBoxCommand command = new CreateBoxCommand(1, "My Box", NotificationSettings.Off,BoxPrivacySettings.InvitationOnly);
            
        //    Storage storage = new Storage();
        //    Box box = storage.CreateBox(command.BoxName);

        //    createBoxCommandHandlerStub.Stub(x => x.Execute(command)).Return(new CreateBoxCommandResult(box));

        //    //Act
        //    CreateBoxCommandResult commandResult = bus.Dispatch<CreateBoxCommand, CreateBoxCommandResult>(command);

        //    //Assert
        //    Assert.AreEqual("My Box", commandResult.NewBox.BoxName);
        //}

        //[Test]  
        //[ExpectedException(ExpectedException = typeof(ResolutionFailedException))]
        //public void Dispatch_CallWithUnregisteredHandler_GetException()
        //{
        //    //Arrange
        //    MockRepository mockRepository = new MockRepository();

        //    //Create Container
        //    UnityContainer container = new UnityContainer();
            
        //    //Create Bus
        //    ICommandBus bus = new CommandBus(container);

        //    //Create Command
        //    CreateBoxCommand command = new CreateBoxCommand(1, "My Box",NotificationSettings.Off,BoxPrivacySettings.InvitationOnly);

        //    //Act
        //    CreateBoxCommandResult commandResult = bus.Dispatch<CreateBoxCommand, CreateBoxCommandResult>(command);            
        //}
    }    
}
