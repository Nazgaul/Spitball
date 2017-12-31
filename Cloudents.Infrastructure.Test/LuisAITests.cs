using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Infrastructure.AI;
using Microsoft.Cognitive.LUIS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cloudents.Infrastructure.Test
{
    [TestClass]
    public class LuisAITests
    {

        Mock<LuisClient> mock = new Mock<LuisClient>(new object[] { "1", "2", false });

        [TestMethod]
        public async Task InterpretStringAsync_SingleWord_ReturnSearchWithUserText()
        {
            LuisAI unit = new LuisAI(mock.Object);

            var result = await unit.InterpretStringAsync("suburbs", default);

            Assert.AreEqual(AiIntent.Search, result.Intent);
            CollectionAssert.AreEqual(new[] { "suburbs" }, result.Subject.ToList());
        }
    }
}
