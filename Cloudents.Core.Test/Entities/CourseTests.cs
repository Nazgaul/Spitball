//using Cloudents.Core.Entities;
//using System;
//using Xunit;

//namespace Cloudents.Core.Test.Entities
//{
//    public class CourseTests
//    {
//        [Theory]
//        [InlineData('a', 3)]
//        [InlineData('a', 151)]
//        public void IllegalCourseNameTests(char c, int num)
//        {
//            string str = new string(c, num);
//            Assert.Throws<ArgumentException>(() => new OldCourse(str));
//        }

//        [Theory]
//        [InlineData('a', 4)]
//        [InlineData('a', 150)]
//        public void CourseNameEdgeTests(char c, int num)
//        {
//            string str = new string(c, num);
//            _ = new OldCourse(str);
//        }

//        //[Fact]
//        //public void NewCourseStateTest()
//        //{
//        //    Course course = new Course("test");
//        //    course.State.Should().Be(ItemState.Pending);
//        //}

//        //[Fact]
//        //public void ApproveCourseTest()
//        //{
//        //    Course course = new Course("test");
//        //    course.Approve();
//        //    course.State.Should().Be(ItemState.Ok);
//        //}
//    }
//}
