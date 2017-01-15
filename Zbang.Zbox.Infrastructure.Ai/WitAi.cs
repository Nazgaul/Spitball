using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class WitAi : IWitAi
    {

        private readonly WitClient m_Client = new WitClient("LRSR7XQUEU5UB7TQNBB64B7BHZDC7UNH");
        public async Task<IIntent> GetUserIntentAsync(string sentence, CancellationToken token)
        {
            var predict = await m_Client.GetMessageAsync(sentence, token);

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
            foreach (var prop in intentObject.GetType().GetProperties())
            {
                List<Entity> entity;
                if (predict.Entities.TryGetValue(prop.Name, out entity))
                {
                    prop.SetValue(intentObject, entity[0].Value);
                }
                if (prop.Name == "SearchQuery" && predict.Entities.TryGetValue("search_query", out entity))
                {
                    prop.SetValue(intentObject, entity[0].Value);
                }
            }
            //if (predict.Entities.TryGetValue("search_query", out entity))
            //{
            //    intentObject.SearchQuery = entity[0].Value;

            //}
            return intentObject;
        }


        public Task AddCoursesEntityAsync(IEnumerable<string> courses,CancellationToken token)
        {
            var tasks = new List<Task>();
            foreach (var course in courses)
            {
                token.ThrowIfCancellationRequested();
                tasks.Add(m_Client.AddEntityValueAsync("CourseName", course, null, null, token));
            }
            return Task.WhenAll(tasks);
        }
        public Task AddUniversitiesEntityAsync(IEnumerable<UniversityEntityDto> universities, CancellationToken token)
        {
            if (universities == null)
            {
                return Extensions.TaskExtensions.CompletedTask;
            }
            var tasks = new List<Task>();
            foreach (var university in universities)
            {
                token.ThrowIfCancellationRequested();
                tasks.Add(m_Client.AddEntityValueAsync("UniversityName", university.Name, university.Extra, university.Id.ToString(), token));
            }
            return Task.WhenAll(tasks);
        }
    }
}
