using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace ConsoleApp
{
    /// <summary>
    /// YouTube Data API v3 sample: search by keyword.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://developers.google.com/api-client-library/dotnet/get_started
    ///
    /// Set ApiKey to the API key value from the APIs & auth > Registered apps tab of
    ///   https://cloud.google.com/console
    /// Please ensure that you have enabled the YouTube Data API for your project.
    /// </summary>
    public class Search
    {
        public void SearchYoutube()
        {
            try
            {
                Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }
        static string[] _scopes = { YouTubeService.Scope.YoutubeReadonly};
        static string _applicationName = "Quickstart";
        private static async Task Run()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token2.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName,
            });
            //var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            //{
            //    ApiKey = "AIzaSyB2pEQZ2STbnKV1xTj4zR0y3bn3D_atpRk",
            //    ApplicationName = "BSNLP"
            //});

            //var x = service.Videos.Insert()


            var searchListRequest = service.Search.List("snippet");
            searchListRequest.Q = "מבוא לפסיכולוגיה שיעור פרטי"; // Replace with your search term.
            searchListRequest.MaxResults = 50;
            searchListRequest.RelevanceLanguage = "he";

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            var str = searchListResponse.Items.Select(s => s.Id.VideoId).First().ToString();
            var vidiosListRequest = service.Videos.List("Id,snippet,contentDetails,topicDetails,statistics,recordingDetails,localizations");
            vidiosListRequest.Id = str;

            var vidiosListResponse = await vidiosListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();
            List<string> playlists = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

            Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));
        }
    }
}