using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cloudents.Infrastructure.Framework.Test
{
    [TestClass]
    public class CourseTests : InMemoryDatabaseTest
    {
        //TODO need to figure out how to do this
        //[TestMethod]
        //public void CanCorrectlyMapCourse()
        //{
        //    var university = new PersistenceSpecification<University>(Session)
        //        .CheckProperty(c => c.Name, "Some University")
        //        .CheckProperty(c => c.Id, 171885L)
        //        .VerifyTheMappings();
        //    //var university = new University(171885,"Some University");
        //    new PersistenceSpecification<Course>(Session)
        //        .CheckProperty(c => c.Name, "TestVitali")
        //        .CheckReference(c => c.University, university)
        //        .VerifyTheMappings();
        //}
    }
}