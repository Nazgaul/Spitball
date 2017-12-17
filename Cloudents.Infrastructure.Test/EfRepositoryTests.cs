using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class EfRepositoryTests
    {
        [TestMethod]
        public async Task AddWritesToDataBaseCourseIsDeletedFalse()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;



            // Run the test against one instance of the context
            using (var context = new AppDbContext(options))
            {
                var service = new EfRepository<Course>(context);
                await service.AddAsync(new Course("ram", 1), default);
            }

            // Use a separate instance of the context to verify correct data was saved to database
            using (var context = new AppDbContext(options))
            {
                Assert.AreEqual(1, context.Courses.Count());
                Assert.AreEqual(false, context.Courses.Single().IsDeleted);
            }
        }
    }
}
