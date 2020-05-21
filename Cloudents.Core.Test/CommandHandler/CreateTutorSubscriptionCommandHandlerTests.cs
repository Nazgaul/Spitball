using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Cloudents.Command.Command;
using Cloudents.Command.CommandHandler;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.CommandHandler
{
    public class CreateTutorSubscriptionCommandHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_NonUS_Error()
        {
            using var mock = AutoMock.GetLoose();
            var tutorMoq = new Mock<Tutor>();
            
          //var tutorMoq =   mock.Mock<Tutor>();
          tutorMoq.Setup(s => s.User.SbCountry).Returns(Country.Israel);

          mock.Mock<ITutorRepository>().Setup(s => s.LoadAsync(It.IsAny<long>(), default))
              .ReturnsAsync(tutorMoq.Object);
            // The AutoMock class will inject a mock IDependency
            // into the SystemUnderTest constructor
            var command = new CreateTutorSubscriptionCommand(1, 500);
            var sut = mock.Create<CreateTutorSubscriptionCommandHandler>();
            await Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(command, default));
        }
    }
}