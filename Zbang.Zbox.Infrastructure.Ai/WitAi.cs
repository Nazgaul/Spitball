using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public interface IWitAi : IAi
    {

    }
    public class WitAi : IWitAi
    {

        private readonly WitClient m_Client = new WitClient("B7CTPD5LNYKZNWZ7UFZMZ4BPV6B4SZW4");
        public async Task<IIntent> GetUserIntentAsync(string sentence)
        {
            var sw = new Stopwatch();
            sw.Start();
            //var client = new WitClient("B7CTPD5LNYKZNWZ7UFZMZ4BPV6B4SZW4");
            var predict = await m_Client.GetMessageAsync(sentence);

            var intent = predict.Intent;
            if (intent == null)
            {
                return null;
            }
            var intentObject = Ioc.IocFactory.IocWrapper.TryResolve<IIntent>(intent.Value);
            if (intentObject == null)
            {
                return null;
            }
            List<Entity> entity;
            //var entities = predict.GetAllEntities();
            foreach (var prop in intentObject.GetType().GetProperties())
            {
                
                if (predict.Entities.TryGetValue(prop.Name, out entity))
                {
                    prop.SetValue(intentObject, entity[0].Value);
                }
                //var entity = entities.Find(f => f.Name == prop.Name);
                //if (entity != null)
                //{
                    
                //}
            }
            if (predict.Entities.TryGetValue("search_query", out entity))
            {
                intentObject.SearchQuery = entity[0].Value;

            }
            sw.Stop();
            intentObject.Time = sw.ElapsedMilliseconds;
            return intentObject;
        }
    }
}
