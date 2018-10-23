using Autofac.Extras.Moq;
using Cloudents.Core.Entities.Db;
using Cloudents.Infrastructure.Database.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace Cloudents.Infrastructure.Test.Database.Repositories
{
    [TestClass]
    public class UserRepositoryTests
    {
        [TestMethod]
        public async Task UserRepository_UserLockedOut_BypassException()
        {
            const long userId = 1L;

            var user = new User("someEmail", "someName", "somePrivateKey")
            {
                LockoutEnd = DateTimeOffset.MaxValue,
                LockoutEnabled = true
            };
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ISession>().Setup(s => s.LoadAsync<User>(userId, default)).ReturnsAsync(user);
                var instance = mock.Create<UserRepository>();

                var result = await instance.LoadAsync(userId, false, default);

                result.Id.Should().Be(userId);
            }
        }
    }
}