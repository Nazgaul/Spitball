using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        Mock<ILuisClient> mock = new Mock<ILuisClient>();

        [TestMethod]
        public async Task InterpretStringAsync_SingleWord_ReturnSearchWithUserText()
        {
            LuisAI unit = new LuisAI(mock.Object);

            var result = await unit.InterpretStringAsync("suburbs", default);

            Assert.AreEqual(AiIntent.Search, result.Intent);
            CollectionAssert.AreEqual(new[] { "suburbs" }, result.Subject.ToList());
        }

        [TestMethod]
        public async Task InterpretStringAsync_ResultNoEntites_ReturnEntityWithUserText()
        {
            const string sentence = "looking for blue";
            mock.Setup(s => s.Predict(sentence)).Returns(Task.FromResult(new LuisResult()
            {
                Entities = new ConcurrentDictionary<string, IList<Entity>>(),
                TopScoringIntent = new Intent()
                {
                    Name = "Ask",
                    Score = 100
                }

            }));
            LuisAI unit = new LuisAI(mock.Object);

            var result = await unit.InterpretStringAsync(sentence, default);

            CollectionAssert.AreEqual(new[] { sentence }, result.Subject.ToList());
        }
    }
}
