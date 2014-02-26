using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Zbox.InfrastructureTests.Security
{
    [TestClass]
    public class UserDetailTest
    {

        [TestMethod]
        public void Serialize_Null_ReturnEmptyString()
        {
            UserDetail userDetail = null;

            var result = UserDetail.Serialize(userDetail);
            Assert.AreSame(result, string.Empty, "Should return Empty string");

        }

        [TestMethod]
        public void Serialize_UserDataWithData_ReturnString()
        {
            //string name = "ram";
            var language = "en";
            //var imageurl = "sss";
            //var uid = "xxx";
            var universityId = 4;
            var universityDataId = 3;
            var scrore = 5;

            UserDetail userDetail = new UserDetail(language, universityId, universityDataId);

            var result = UserDetail.Serialize(userDetail);

            var expectedResult = language + "@" + scrore + "@" + universityId + "@" + universityDataId;
            var thesame = result == expectedResult;
            Assert.IsTrue(thesame, "should be the same");
        }

        [TestMethod]
        public void Deserialize_EmptyString_ReturnNull()
        {
            var data = string.Empty;
            var userDetail = UserDetail.Deserialize(data);
            Assert.IsNull(userDetail, "Should return null");
        }

        [TestMethod]
        public void Deserialize_UserWithData_ReturnUserDetail()
        {
            //string name = "ram";
            var language = "en";
            //var imageurl = "sss";
            //var uid = "xxx";
            var universityId = 4;
            var universityDataId = 3;
            var score = 5;

            var data = language + "@" + score + "@" + universityId + "@" + universityDataId;

            var userDetail = UserDetail.Deserialize(data);
            UserDetail userExpectedResult = new UserDetail(language, universityId, universityDataId);

            //Assert.AreEqual(userExpectedResult.Name, userDetail.Name, "Name should be the same");
            Assert.AreEqual(userExpectedResult.UniversityId, userDetail.UniversityId, "university should be the same");
            Assert.AreEqual(userExpectedResult.Language, userDetail.Language, "Language should be the same");
            Assert.AreEqual(userExpectedResult.UniversityWrapperId, userDetail.UniversityWrapperId, "UniversityDataId should be the same");
            //Assert.AreEqual(userExpectedResult.Uid, userDetail.Uid, "Uid should be the same");
        }

        [TestMethod]
        public void Deserialize_UserWithMoreDelimiters_ReturnNull()
        {
            string name = "ram";
            var data = name + "@@@" + "true";
            var userDetail = UserDetail.Deserialize(data);
            Assert.IsNull(userDetail, "Should be null");
        }

        //[TestMethod]
        //[ExpectedException(typeof(FormatException))]
        //public void Deserialize_VerifiedIsNotBool_RaiseException()
        //{
        //    string name = "ram";
        //    var data = name + "@" + "test" + "@" + "en" + "@" + "xxx" + "@" + "vvv"; ;
        //    var userDetail = UserDetail.Deserialize(data);

        //}
    }
}
