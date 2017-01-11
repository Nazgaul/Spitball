﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class WitAi : IWitAi
    {

        private readonly WitClient m_Client = new WitClient("B7CTPD5LNYKZNWZ7UFZMZ4BPV6B4SZW4");
        public async Task<IIntent> GetUserIntentAsync(string sentence, CancellationToken token)
        {
            var sw = new Stopwatch();
            sw.Start();
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
