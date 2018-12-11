using Autofac.Extras.Moq;
using Cloudents.Core.Entities.Db;
using Cloudents.Infrastructure.Database.Repositories;
using Moq;
using NHibernate;
using System;
using System.Globalization;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Cloudents.Infrastructure.Test.Database.Repositories
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task UserRepository_UserLockedOut_BypassException()
        {
            const long userId = 1L;

            var user = new RegularUser("someEmail", "someName", "somePrivateKey", CultureInfo.InvariantCulture)
            {
                LockoutEnd = DateTimeOffset.MaxValue,
                LockoutEnabled = true
            };
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<ISession>().Setup(s => s.LoadAsync<User>(userId, default)).ReturnsAsync(user);
                var instance = mock.Create<RegularUserRepository>();

                var result = await instance.LoadAsync(userId, false, default);

                result.Id.Should().Be(userId);
            }
        }
    }
}