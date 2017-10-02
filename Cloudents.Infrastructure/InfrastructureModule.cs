﻿using System;
using System.Collections.Generic;
using Autofac;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure
{
    public class InfrastructureModule : Module
    {
        private readonly string m_SqlConnectionString;
        private readonly string m_SearchServiceName;
        private readonly string m_SearchServiceKey;

        public InfrastructureModule(string sqlConnectionString, string searchServiceName, string searchServiceKey)
        {
            m_SqlConnectionString = sqlConnectionString;
            m_SearchServiceName = searchServiceName;
            m_SearchServiceKey = searchServiceKey;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LuisAI>().As<IAI>();
            builder.RegisterType<AIDecision>().As<IDecision>();
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();

            builder.Register(c => new DapperRepository(m_SqlConnectionString));
            builder.Register(c =>
                new SearchServiceClient(m_SearchServiceName, new SearchCredentials(m_SearchServiceKey))).SingleInstance();

            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>().PropertiesAutowired();
            builder.RegisterType<FlashcardSearch>().As<IFlashcardSearch>().PropertiesAutowired();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>().PropertiesAutowired();
            builder.RegisterType<TutorSearch>().As<ITutorSearch>();
            builder.RegisterType<TitleSearch>().As<ITitleSearch>();
            builder.RegisterType<VideoSearch>().As<IVideoSearch>();
            builder.RegisterType<JobSearch>().As<IJobSearch>();
            builder.RegisterType<BookSearch>().As<IBookSearch>();

            builder.RegisterType<UniversitySynonymRepository>().As<IReadRepositorySingle<UniversitySynonymDto, long>>();

            MapperConfiguration(builder);
        }

        private static void MapperConfiguration(ContainerBuilder builder)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Search.Entities.Tutor, TutorDto>()
                    .ForMember(d => d.Location, o => o.ResolveUsing(p => new GeoPoint
                    {
                        Latitude = p.Location.Latitude,
                        Longitude = p.Location.Longitude
                    }))
                    .ForMember(d => d.TermFound, o => o.ResolveUsing((t, ts, i, c) =>
                    {
                        var temp = $"{t.City} {t.State} {string.Join(" ", t.Subjects)} {string.Join(" ", t.Extra)}";
                        return temp.Split(new[] {c.Items["term"].ToString()},
                            StringSplitOptions.RemoveEmptyEntries).Length;
                    }));

                cfg.CreateMap<Search.Entities.Job, JobDto>();
                cfg.CreateMap<JToken, BookSearchDto>().ConvertUsing((jo, bookSearch) => jo.ToObject<BookSearchDto>());
                cfg.CreateMap<JObject, IEnumerable<BookSearchDto>>().ConvertUsing((jo, bookSearch) => jo["response"]["page"]["results"]["book"].ToObject<IEnumerable<BookSearchDto>>());
            });
            builder.Register(c => config.CreateMapper()).SingleInstance();
        }
    }
}
