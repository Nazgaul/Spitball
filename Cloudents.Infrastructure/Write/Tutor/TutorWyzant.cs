using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Write.Tutor.Entities;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Write.Tutor
{
    public class TutorWyzant : UpdateAffiliate<WyzantTutor, Core.Entities.Search.Tutor>, IDisposable
    {
        private readonly TutorSearchWrite _tutorProvider;
        private readonly IGooglePlacesSearch _zipToLocation;

        public TutorWyzant(TutorSearchWrite tutorProvider, ILogger logger,
            IGooglePlacesSearch zipToLocation, IDownloadFile localStorage)
            : base(logger, localStorage)
        {
            _tutorProvider = tutorProvider;
            _zipToLocation = zipToLocation;
            HttpHandler = new HttpClientHandler();
        }

        protected override string FileLocation => "tutor.json";

        protected override Uri Url =>
            new Uri("https://data.wyzant.com/feeds/downloadFeed?apiKey=286f1896-e056-4376-9747-9f9a5dbcb4d2&feedFormat=json&MaxResults=1");

        protected override string Service => "WyzantTutor";
        protected override HttpClientHandler HttpHandler { get; }


        private const string Source = "Wyzant";
        protected override IEnumerable<WyzantTutor> GetT(string location)
        {
            var serializer = new JsonSerializer();
            using (var s = File.Open(location, FileMode.Open))
            using (var sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    // deSerialize only when there's "{" character in the stream
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var o = serializer.Deserialize<WyzantTutor>(reader);
                        yield return o;
                    }
                }
            }
        }

        protected override async Task<Core.Entities.Search.Tutor> ParseTAsync(WyzantTutor obj, CancellationToken token)
        {
            var filter = TutorFilter.None;
            if (obj.OffersInPersonLessons)
            {
                filter |= TutorFilter.InPerson;
            }

            if (obj.OffersOnlineLessons)
            {
                filter |= TutorFilter.Online;
            }
            var location = await _zipToLocation.GeoCodingByZipAsync(obj.Zip, token).ConfigureAwait(false);
            return new Core.Entities.Search.Tutor
            {
                Id = obj.TutorID.ToString(),
                Name = obj.Name,
                Image = obj.TutorPictures.FirstOrDefault(),
                Url = obj.ProfileLink,
                Description = obj.Title,
                City = obj.City,
                State = obj.State,
                Location = GeoPoint.ToPoint(location),
                Fee = obj.FeePerHour,
                TutorFilter = filter,
                InsertDate = DateTime.UtcNow,
                Extra = obj.Subjects?.Select(s => s.Name).ToArray(),
                Source = Source
            };
        }

        protected override Task UpdateSearchAsync(IEnumerable<Core.Entities.Search.Tutor> list, CancellationToken token)
        {
            return _tutorProvider.UpdateDataAsync(list, token);
        }

        protected override Task DeleteOldItemsAsync(DateTime timeToDelete, CancellationToken token)
        {
            return _tutorProvider.DeleteOldTutorsAsync(Source, timeToDelete, token);
        }

        public void Dispose()
        {
            HttpHandler?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
