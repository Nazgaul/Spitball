using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Cloudents.Core.Test
{
    [TestClass]
    public class SignalRTransportTypeTests
    {
        [TestMethod]
        public void Serialize_SignalRTransportType_RightJson()
        {
            var elem = new SignalRTransportType(SignalRType.Question,SignalRAction.Add,null);
            var content = JsonConvert.SerializeObject(elem);
            var expectedJson = "{\"type\":\"question\",\"action\":\"add\"}";
            content.Should().BeEquivalentTo(expectedJson);
        }

        //[TestMethod]
        //public void Serialize_SignalRTransportType_RightJson2()
        //{
        //    var dto = new QuestionFeedDto
        //    {
        //        User = new UserDto
        //        {
        //            Id = 638,
        //            Name = "yaari.9181",
        //            Image = null
        //        },
        //        Answers = 0,
        //        Id = 2827,
        //        DateTime = DateTime.Parse("2018-10-20T23:09:18.486Z"),
        //        Files = 0,
        //        HasCorrectAnswer = false,
        //        Price = 10,
        //        Text = "54645 6ryt yfthfhgf hft 54 ryt5656 rytyrtyrty",
        //        Color = QuestionColor.Default,
        //        Subject = QuestionSubject.Biology
        //    };

        //    var singleQuestion =
        //        "{\"id\":2827,\"subject\":\"biology\",\"price\":10.0,\"text\":\"54645 6ryt yfthfhgf hft 54 ryt5656 rytyrtyrty\",\"files\":0,\"answers\":0,\"user\":{\"id\":638,\"name\":\"yaari.9181\"},\"dateTime\":\"2018-10-20T23:09:18.486Z\",\"color\":\"default\",\"hasCorrectAnswer\":false}";
        //    var expectedJson = "{\"type\":\"question\",\"action\":\"add\",\"data\":[" + singleQuestion + "]}";
        //    var elem = new SignalRTransportType(SignalRType.Question, SignalRAction.Add, dto);

        //    var content = JsonConvert.SerializeObject(elem);
        //    content.Should().BeEquivalentTo(expectedJson);

        //}
    }
}