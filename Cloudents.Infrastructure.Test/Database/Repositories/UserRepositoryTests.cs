//using Autofac.Extras.Moq;
//using Moq;
//using NHibernate;
//using System;
//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;
//using Autofac.Core;
//using Cloudents.Core.Entities;
//using Cloudents.Persistance.Repositories;
//using FluentAssertions;
//using Xunit;

//namespace Cloudents.Infrastructure.Test.Database.Repositories
//{
//    //public class UserRepositoryTests
//    //{
//    //    [Fact]
//    //    public async Task UserRepository_UserLockedOut_BypassException()
//    //    {
//    //        const long userId = 1L;

//    //        var user = new RegularUser("someEmail", "someName", "somePrivateKey", CultureInfo.InvariantCulture)
//    //        {
//    //            LockoutEnd = DateTimeOffset.MaxValue,
//    //            LockoutEnabled = true
//    //        };
//    //        using (var mock = AutoMock.GetLoose())
//    //        {
//    //            mock.Mock<ISession>().Setup(s => s.LoadAsync<User>(userId, default)).ReturnsAsync(user);
//    //            var instance = mock.Create<RegularUserRepository>();

//    //            var result = await instance.LoadAsync(userId, false, default);

//    //            result.Id.Should().Be(userId);
//    //        }
//    //    }
//    //}

//    public class CoursesRepositoryTests
//    {
//        [Fact]
//        public async Task GetOrAddAsync_NameWithSpacesAtEnd_CourseTrim()
//        {
//            using (var mock = AutoMock.GetLoose())
//            {
//                var input = "Hadar";
//                var courseName = "Hadar";
//                var courseId = 1L;

//                var sessionMock = mock.Mock<ISession>();
//                var courseMoq = new Mock<Course>(sessionMock.Object);
//                //var courseMoq = mock.Mock<Course>(new Parameter[] {sessionMock.Object});
//                courseMoq.Setup(s => s.Id).Returns(courseId);
//                courseMoq.Setup(s => s.Name).Returns(courseName);



//                sessionMock.Setup(x => x.Query<Course>())
//                    .Returns(new EnumerableQuery<Course>(new [] {courseMoq.Object}));





//                sessionMock.Setup(x => x.GetAsync<Course>(courseId, default)).ReturnsAsync(courseMoq.Object);

//                var courseRepositoryStub = mock.Mock<CourseRepository>();

//                var result = await courseRepositoryStub.Object.GetOrAddAsync(input, default);

//                result.Should().Be(courseMoq.Object);
//            }
//        }
//    }
//}