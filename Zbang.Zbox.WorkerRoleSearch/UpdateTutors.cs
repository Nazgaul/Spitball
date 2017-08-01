using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.WorkerRoleSearch.Mail;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class UpdateTutors : ISchedulerProcess
    {
        private readonly TutorProvider m_TutorProvider;
        private readonly ILogger m_Logger;
        private readonly IZipToLocationProvider m_ZipToLocation;
        private readonly ILocalStorageProvider m_LocalStorage;

        public UpdateTutors(TutorProvider tutorProvider, ILogger logger, IZipToLocationProvider zipToLocation, ILocalStorageProvider localStorage)
        {
            m_TutorProvider = tutorProvider;
            m_Logger = logger;
            m_ZipToLocation = zipToLocation;
            m_LocalStorage = localStorage;
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            if (progressAsync == null) throw new ArgumentNullException(nameof(progressAsync));
            m_Logger.Info("Update tutors starting to work");
            string locationToSave;
            using (var client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",)
                var result = await client.GetAsync("https://data.wyzant.com/feeds/downloadFeed?apiKey=286f1896-e056-4376-9747-9f9a5dbcb4d2&feedFormat=json",
                    HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
                result.EnsureSuccessStatusCode();



                using (var stream = await result.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    locationToSave = await m_LocalStorage.SaveFileToStorageAsync(stream, "tutor.json", index == 0)
                        .ConfigureAwait(false);
                }
            }
            var list = new List<Tutor>();
            var i = 0;
            foreach (var tutor in GetTutor(locationToSave))
            {
                if (i < index)
                {
                    continue;
                }
                i++;
                var jobObject = await m_TutorProvider.GetByIdAsync(tutor.TutorID.ToString(), token).ConfigureAwait(false);

                if (jobObject == null)
                {
                    jobObject = await CreateTutorObjectAsync(tutor).ConfigureAwait(false);

                }
                else if (jobObject.Location == null && !string.IsNullOrEmpty(tutor.Zip))
                {
                    jobObject = await CreateTutorObjectAsync(tutor).ConfigureAwait(false);
                }
                else if (jobObject.InsertDate > DateTime.UtcNow.AddDays(-1))
                {
                    continue;
                }
                jobObject.InsertDate = DateTime.UtcNow;
                list.Add(jobObject);
                if (list.Count > 100)
                {
                    var t1 = m_TutorProvider.UpdateDataAsync(list, token);
                    var t2 = progressAsync.Invoke(i, TimeSpan.FromMinutes(10));
                    m_Logger.Info("Update jobs finish processing " + i);

                    await Task.WhenAll(t1, t2).ConfigureAwait(false);
                    token.ThrowIfCancellationRequested();
                    list.Clear();
                }


            }
            if (list.Count > 0)
            {
                await m_TutorProvider.UpdateDataAsync(list, token).ConfigureAwait(false);
            }
            m_Logger.Info("Update jobs Going To delete old jobs");
            var oldJobs = await m_TutorProvider.GetOldTutorsAsync(token).ConfigureAwait(false);

            await m_TutorProvider.DeleteDataAsync(oldJobs, token).ConfigureAwait(false);
            m_Logger.Info("Update jobs finish to work");
            return true;

        }

        private async Task<Tutor> CreateTutorObjectAsync(WyzantTutor tutor)
        {
            var location = await m_ZipToLocation.GetLocationViaZipAsync(tutor.Zip).ConfigureAwait(false);
            var searchJobObject = new Tutor
            {
                City = tutor.City,
                Fee = tutor.FeePerHour,
                Id = tutor.TutorID.ToString(),
                Image = tutor.TutorPictures.FirstOrDefault(),
                InPerson = tutor.OffersInPersonLessons,
                InsertDate = DateTime.UtcNow,
                Location = location,
                Name = tutor.Name,
                Online = tutor.OffersOnlineLessons,
                Rank = tutor.TutorRank,
                State = tutor.State,
                Subjects = tutor.Subjects?.Select(s=>s.Name).ToArray(),
                Url = tutor.ProfileLink
            };
            return searchJobObject;
        }

        private IEnumerable<WyzantTutor> GetTutor(string location)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (FileStream s = File.Open(location, FileMode.Open))
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    // deserialize only when there's "{" character in the stream
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        var o = serializer.Deserialize<WyzantTutor>(reader);
                        yield return o;
                    }
                }
            }

        }

    }
}
