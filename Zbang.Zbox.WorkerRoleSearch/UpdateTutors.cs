using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.Spatial;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Search;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateTutors : UpdateAffiliate<WyzantTutor, Tutor>
    {
        private readonly TutorProvider _tutorProvider;
        private readonly IGooglePlacesSearch _zipToLocation;

        public UpdateTutors(TutorProvider tutorProvider, ILogger logger, IGooglePlacesSearch zipToLocation, ITempStorageProvider localStorage)
            : base(logger, localStorage)
        {
            _tutorProvider = tutorProvider;
            _zipToLocation = zipToLocation;
        }

        protected override string FileLocation => "tutor.json";

        protected override string Url =>
            "https://data.wyzant.com/feeds/downloadFeed?apiKey=286f1896-e056-4376-9747-9f9a5dbcb4d2&feedFormat=json";

        protected override string Service => "WyzantTutor";

        protected override IEnumerable<WyzantTutor> GetT(string location)
        {
            var serializer = new JsonSerializer();
            using (FileStream s = File.Open(location, FileMode.Open))
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

        protected override async Task<Tutor> ParseTAsync(WyzantTutor obj, CancellationToken token)
        {
            var location = await _zipToLocation.GeoCodingByZipAsync(obj.Zip, token).ConfigureAwait(false);
            return new Tutor
            {
                City = obj.City,
                Fee = obj.FeePerHour,
                Id = obj.TutorID.ToString(),
                Image = obj.TutorPictures.FirstOrDefault(),
                InPerson = obj.OffersInPersonLessons,
                InsertDate = DateTime.UtcNow,
                Location = GeographyPoint.Create(location.Latitude, location.Longitude),
                Name = obj.Name,
                Online = obj.OffersOnlineLessons,
                Rank = obj.TutorRank,
                State = obj.State,
                Description = obj.Title,
                Subjects = obj.Subjects?.Select(s => s.Name).ToArray(),
                Url = obj.ProfileLink
            };
        }

        protected override Task UpdateSearchAsync(IEnumerable<Tutor> list, CancellationToken token)
        {
            return _tutorProvider.UpdateDataAsync(list, token);
        }

        protected override Task DeleteOldItemsAsync(CancellationToken token)
        {
            return _tutorProvider.DeleteOldTutorsAsync(token);
        }
    }
}
