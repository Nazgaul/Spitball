using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Cognitive.LUIS;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public interface ILuisAi
    {
        Task<IIntent> GetUserIntentAsync(string sentence);
    }

    public class LuisAi : ILuisAi, IDisposable
    {
        private readonly LuisClient m_Luisclient =
                new LuisClient("c097768f-836d-4102-a16e-11f4c11e35a9",
                    "6effb3962e284a9ba73dfb57fa1cfe40");


        public async Task<IIntent> GetUserIntentAsync(string sentence)
        {
            var predict = await m_Luisclient.Predict(sentence);
            var intent = predict.TopScoringIntent.Name;
            var intentObject = Ioc.IocFactory.IocWrapper.TryResolve<IIntent>(intent);
            var entities = predict.GetAllEntities();
            foreach (var prop in intentObject.GetType().GetProperties())
            {
                var entity = entities.Find(f => f.Name == prop.Name);
                if (entity != null)
                {
                    prop.SetValue(intentObject, entity.Value);
                }
            }
            return intentObject;

        }

        public void Dispose()
        {
            m_Luisclient?.Dispose();
        }
    }

    public interface IIntent
    {
        
    }
    public class HomeWorkIntent : IIntent
    {
        public string UniversityName { get; set; }
        public string CourseName { get; set; }

        public string SearchQuery { get; set; }
    }
}
