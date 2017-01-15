using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Cognitive.LUIS;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public interface ILuisAi: IAi
    {
        
    }

    public class LuisAi : ILuisAi, IDisposable
    {
        private readonly LuisClient m_Luisclient =
                new LuisClient("c097768f-836d-4102-a16e-11f4c11e35a9",
                    "6effb3962e284a9ba73dfb57fa1cfe40");


        public async Task<IIntent> GetUserIntentAsync(string sentence, CancellationToken token)
        {
            //var sw = new Stopwatch();
            //sw.Start();
            var predict = await m_Luisclient.Predict(sentence);
            var intent = predict.TopScoringIntent.Name;
            var intentObject = Ioc.IocFactory.IocWrapper.TryResolve<IIntent>(intent);
            if (intentObject == null)
            {
                return null;
            }
            var entities = predict.GetAllEntities();
            foreach (var prop in intentObject.GetType().GetProperties())
            {
                var entity = entities.Find(f => f.Name == prop.Name);
                if (entity != null)
                {
                    prop.SetValue(intentObject, entity.Value);
                }
            }
            //sw.Stop();
            //intentObject.Time = sw.ElapsedMilliseconds;
            
            return intentObject;

        }

        public void Dispose()
        {
            m_Luisclient?.Dispose();
        }
    }
}
