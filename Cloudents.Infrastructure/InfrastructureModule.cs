using System;
using System.Collections.Generic;
using System.Linq;
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
            builder.RegisterType<RestClient>().As<IRestClient>();
            builder.RegisterType<PurchaseSearch>().As<IPurchaseSearch>();

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
                        return temp.Split(new[] { c.Items["term"].ToString() },
                            StringSplitOptions.RemoveEmptyEntries).Length;
                    }));

                cfg.CreateMap<Search.Entities.Job, JobDto>();
                cfg.CreateMap<JObject, IEnumerable<BookSearchDto>>().ConvertUsing((jo, bookSearch,c) =>
                {
                    return jo["response"]["page"]["books"]["book"].Select(json =>
                    {
                        return c.Mapper.Map<JToken, BookSearchDto>(json);
                        //return new BookSearchDto
                        //{
                        //    Image = json["image"]?["image"].Value<string>(),
                        //    Author = json["author"].Value<string>(),
                        //    Binding = json["binding"].Value<string>(),
                        //    Edition = json["edition"].Value<string>(),
                        //    Isbn10 = json["isbn10"].Value<string>(),
                        //    Isbn13 = json["isbn13"].Value<string>(),
                        //    Title = json["title"].Value<string>()
                        //};
                    });
                });
                cfg.CreateMap<JToken, BookSearchDto>().ConvertUsing((jo, bookSearch) => new BookSearchDto
                {
                    Image = jo["image"]?["image"].Value<string>(),
                    Author = jo["author"].Value<string>(),
                    Binding = jo["binding"].Value<string>(),
                    Edition = jo["edition"].Value<string>(),
                    Isbn10 = jo["isbn10"].Value<string>(),
                    Isbn13 = jo["isbn13"].Value<string>(),
                    Title = jo["title"].Value<string>()

                });
                cfg.CreateMap<JObject, BookDetailsDto>().ConvertUsing((jo, bookSearch, c) =>
                {

                    var book = jo["response"]["page"]["books"]["book"].First;
                    var offers = book["offers"]["group"].SelectMany(json =>
                    {
                        return json["offer"].Select(s => new BookPricesDto
                        {
                            Condition = s["condition"]["condition"].Value<string>(),
                            Image = s["merchant"]["image"].Value<string>(),
                            Link = s["link"].Value<string>(),
                            Name = s["merchant"]["name"].Value<string>(),
                            Price = s["total"].Value<double>()
                        });
                    });
                    return new BookDetailsDto
                    {

                        Details = c.Mapper.Map<JToken, BookSearchDto>(book), //book.ToObject<BookSearchDto>(),,
                        Prices = offers
                    };
                });
                cfg.CreateMap<JObject, IEnumerable<PlaceDto>>().ConvertUsing((jo, bookSearch, c) =>
                {
                    return jo["results"].Select(json =>
                    {
                        var photo = json["photos"]?[0]?["photo_reference"]?.Value<string>();
                        string image = null;
                        if (!string.IsNullOrEmpty(photo))
                        {
                            image =
                                $"https://maps.googleapis.com/maps/api/place/photo?maxwidth={c.Items["width"]}&photoreference={photo}&key={c.Items["key"]}";
                        }
                        GeoPoint location = null;
                        if (json["geometry"]?["location"] != null)
                        {
                            location = new GeoPoint
                            {
                                Latitude = json["geometry"]["location"]["lat"].Value<double>(),
                                Longitude = json["geometry"]["location"]["lng"].Value<double>()
                            };
                        }
                        return new PlaceDto
                        {
                            Address = json["vicinity"].Value<string>(),
                            Image = image,
                            Location = location,
                            Name = json["name"].Value<string>(),
                            Open = json["opening_hours"]?["open_now"]?.Value<bool>() ?? false,
                            Rating = json["rating"]?.Value<double>() ?? 0
                        };
                    });
                });
                cfg.CreateMap<JObject, IEnumerable<TutorDto>>().ConvertUsing((jo, tu, c) =>
                {

                    return jo["results"].Children().Select(result => new TutorDto
                    {
                        Url = $"https://tutorme.com/tutors/{result["id"].Value<string>()}",
                        Image = result["avatar"]["x300"].Value<string>(),
                        Name = result["shortName"].Value<string>(),
                        Online = result["isOnline"].Value<bool>(),
                        TermFound = result.ToString().Split(new[] { c.Items["term"].ToString() },
                            StringSplitOptions.RemoveEmptyEntries).Length
                    });
                });
            });
            builder.Register(c => config.CreateMapper()).SingleInstance();
        }
    }
}
