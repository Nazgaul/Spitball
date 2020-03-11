using Cloudents.Core.Interfaces;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;

namespace Cloudents.Infrastructure.Video
{
    public class MediaServices : IVideoService
    {
        // public const string JobLabelImage = "image";
        // public const string JobLabelShortVideo = "video-short";
        // public const string JobLabelFullVideo = "video";

        private readonly AsyncLazy<AzureMediaServicesClient> _context;
        private ConfigWrapper _config;

        public const string PrefixThumbnailBlobName = "Thumbnail";

        public MediaServices(bool isDevelop)
        {

            _context = new AsyncLazy<AzureMediaServicesClient>(async () => await Init(isDevelop));
        }


        private const string ThumbnailTransformer = "PreviewTransformer";
        private const string StreamingTransformer = "StreamingTransformer";
        private const string FullHdTransformer = "FullHdTransformer";

        public async Task CreateStudyRoomSessionEncoding(string sessionId, string url,
            CancellationToken token)
        {
            var thumbnailAsset = await CreateOutputAssetAsync(sessionId, AssetType.StudyRoom, token);
            var jobInput =
                new JobInputHttp(files: new[] { url });
            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(thumbnailAsset.Name, label: AssetType.StudyRoom.ToString())
            };
            var v = await _context;

            try
            {
                await v.Jobs.CreateAsync(
                    _config.ResourceGroup,
                    _config.AccountName,
                    FullHdTransformer,
                    $"{sessionId} study room session",
                    new Job
                    {
                        Input = jobInput,
                        Outputs = jobOutputs,
                    }, token);
            }
            catch (ApiErrorException e) when (e.Response.StatusCode == HttpStatusCode.Conflict)
            {
                //Do nothing
            }
        }

        private async Task<AzureMediaServicesClient> Init(bool isDevelop)
        {
            var jsonFile = "appsettings.json";

            if (isDevelop)
            {
                jsonFile = "appsettings-dev.json";
            }

            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"Cloudents.Infrastructure.Video.{jsonFile}"))
            {
                _config = stream.ToJsonReader<ConfigWrapper>();
            }


            var clientCredential = new ClientCredential(_config.AadClientId, _config.AadSecret);
            var serviceClientCredentials = await ApplicationTokenProvider.LoginSilentAsync(_config.AadTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure);
            //TODO Maybe we should pass http client factory
            var client = new AzureMediaServicesClient(_config.ArmEndpoint, serviceClientCredentials)
            {
                SubscriptionId = _config.SubscriptionId,

            };

            var t1 = client.Transforms.CreateOrUpdateAsync(
                _config.ResourceGroup,
                _config.AccountName,
                ThumbnailTransformer,
                new List<TransformOutput>()
                {
                    new TransformOutput(
                        new StandardEncoderPreset(
                            new List<Codec>()
                            {
                                new JpgImage(start:"{Best}",layers:new List<JpgLayer>()
                                {
                                    new JpgLayer("100%","100%",quality:80)
                                })
                            }
                            ,
                            new List<Format>()
                            {
                                new JpgFormat(PrefixThumbnailBlobName+"{Index}{Extension}")
                            }),
                        onError:OnErrorType.ContinueJob
                    )
                });
            var t2 = client.Transforms.CreateOrUpdateAsync(
                _config.ResourceGroup,
                _config.AccountName,
                StreamingTransformer,
                new List<TransformOutput>()
                {
                    new TransformOutput(new BuiltInStandardEncoderPreset()
                    {
                        PresetName = EncoderNamedPreset.AdaptiveStreaming
                    }),

                });

            var t3 = client.Transforms.CreateOrUpdateAsync(
                _config.ResourceGroup,
                _config.AccountName,
                FullHdTransformer,
                new List<TransformOutput>()
                {
                    new TransformOutput(new BuiltInStandardEncoderPreset()
                    {
                        PresetName = EncoderNamedPreset.H264SingleBitrate1080p
                    }),

                });

            await Task.WhenAll(t1, t2, t3);
            return client;
            //TODO: need to add event to queue




        }

        public Task CreateAudioPreviewJobAsync(long id, string url, CancellationToken token)
        {
            return CreateStreamingJobAsync(id, url, token);
        }


        public Task CreateVideoPreviewJobAsync(long id, string url, CancellationToken token)
        {
            // This example shows how to encode from any HTTPs source URL - a new feature of the v3 API.  
            // Change the URL to any accessible HTTPs URL or SAS URL from Azure.

            var t1 = CreateThumbnailJobAsync(id, url, token);
            var t2 = CreateStreamingJobAsync(id, url, token);

            return Task.WhenAll(t1, t2);
            //return job;
        }

        public Task DeleteImageAssetAsync(long id, CancellationToken token)
        {
            var assetName = BuildAssetName(id.ToString(), AssetType.Thumbnail);
            return DeleteAssetAsync(assetName, token);
            //var assetName = BuildAssetName(id, AssetType.Thumbnail);
            //await v.Assets.DeleteAsync(_config.ResourceGroup, _config.AccountName, assetName, token);
        }

        public async Task DeleteAssetAsync(string assetName, CancellationToken token)
        {
            var v = await _context;


            //TODO need to delete the jobs as well.
            //TODO need to delete this when the file is deleted
            var result = await v.StreamingLocators.ListAsync(_config.ResourceGroup, _config.AccountName, cancellationToken: token);
            var links = result.Where(w => w.AssetName == assetName);
            foreach (var streamingLocator in links)
            {
                await v.StreamingLocators.DeleteAsync(_config.ResourceGroup, _config.AccountName,
                    streamingLocator.Name, cancellationToken: token);
            }


            await v.Assets.DeleteAsync(_config.ResourceGroup, _config.AccountName, assetName, token);
        }


        public Task DeleteAssetAsync(long id, AssetType type, CancellationToken token)
        {
            var assetName = BuildAssetName(id.ToString(), type);
            return DeleteAssetAsync(assetName, token);
        }


        private async Task CreateThumbnailJobAsync(long id, string url, CancellationToken token)
        {
            var thumbnailAsset = await CreateOutputAssetAsync(id.ToString(), AssetType.Thumbnail, token);
            var jobInput =
                new JobInputHttp(files: new[] { url });
            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(thumbnailAsset.Name, label: AssetType.Thumbnail.ToString())
            };
            var v = await _context;


            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, Get methods on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            try
            {
                await v.Jobs.CreateAsync(
                    _config.ResourceGroup,
                    _config.AccountName,
                    ThumbnailTransformer,
                    $"{id} thumbnail",
                    new Job
                    {
                        Input = jobInput,
                        Outputs = jobOutputs,
                    }, token);
            }
            catch (ApiErrorException e) when (e.Response.StatusCode == HttpStatusCode.Conflict)
            {
                //Do nothing
            }
        }



        public async Task CreatePreviewJobAsync(long id, string url, TimeSpan videoLength, CancellationToken token)
        {

            var clipEndTime = Math.Min(TimeSpan.FromSeconds(30).Ticks, videoLength.Ticks);
            var videoAsset = await CreateOutputAssetAsync(id.ToString(), AssetType.Short, token);
            var jobInput =
                new JobInputHttp(files: new[] { url }, end: new AbsoluteClipTime(new TimeSpan(clipEndTime)));

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(videoAsset.Name, label: AssetType.Short.ToString()),
            };

            var v = await _context;

            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, Get methods on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            try
            {
                await v.Jobs.CreateAsync(
                    _config.ResourceGroup,
                    _config.AccountName,
                    StreamingTransformer,
                    $"{id} preview",
                    new Job
                    {
                        Input = jobInput,
                        Outputs = jobOutputs,
                    }, token);
            }
            catch (ApiErrorException e) when (e.Response.StatusCode == HttpStatusCode.Conflict)
            {
                //There is already a job on this

            }

        }

        private async Task CreateStreamingJobAsync(long id, string url, CancellationToken token)
        {
            var videoAsset = await CreateOutputAssetAsync(id.ToString(), AssetType.Long, token);
            var jobInput =
                new JobInputHttp(files: new[] { url });

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(videoAsset.Name, label: AssetType.Long.ToString()),
            };

            var v = await _context;

            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, Get methods on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            try
            {
                await v.Jobs.CreateAsync(
                    _config.ResourceGroup,
                    _config.AccountName,
                    StreamingTransformer,
                    $"{id} video",
                    new Job
                    {
                        Input = jobInput,
                        Outputs = jobOutputs,
                    }, token);

            }
            catch (ApiErrorException e) when (e.Response.StatusCode == HttpStatusCode.Conflict)
            {
                //There is already a job on this

            }
        }


        private async Task<Asset> CreateOutputAssetAsync(string id, AssetType assetType, CancellationToken token)
        {
            var assetName = BuildAssetName(id, assetType);
            var v = await _context;
            var asset = new Asset(name: assetName, description: assetName, container: assetName, alternateId: id);
            var outputAssetName = assetName;
            return await v.Assets.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName,
                outputAssetName, asset, token);
        }


        public async Task<string> GetAssetContainerAsync(long id, AssetType assetType, CancellationToken token)
        {
            var v = await _context;

            var assetName = BuildAssetName(id.ToString(), assetType);
            var asset = await v.Assets.GetAsync(_config.ResourceGroup, _config.AccountName, assetName, token);

            return asset?.Container;
        }

        private static string BuildAssetName(string id, AssetType assetType)
        {
            return $"{assetType}{id}";

        }

        public async Task RemoveUnusedStreamingLocatorAsync(CancellationToken token)
        {
            var client = await _context;
            var result = await client.StreamingLocators.ListAsync(_config.ResourceGroup,
                _config.AccountName, cancellationToken: token);

            var links = result
                .Where(w => w.EndTime < DateTime.UtcNow);

            var tasks = new List<Task>();
            foreach (var streamingLocator in links)
            {
                var t = client.StreamingLocators.DeleteAsync(_config.ResourceGroup, _config.AccountName,
                    streamingLocator.Name, cancellationToken: token);
                tasks.Add(t);
            }

            await Task.WhenAll(tasks);
        }

        private async Task RemoveUnusedStreamingLocatorAsync(long videoId, CancellationToken token)
        {
            var client = await _context;

            var assetName = BuildAssetName(videoId.ToString(), AssetType.Long);

            var result = await client.StreamingLocators.ListAsync(_config.ResourceGroup,
                _config.AccountName, cancellationToken: token);
            var links = result
                .Where(w => w.AssetName == assetName
                            && w.EndTime < DateTime.UtcNow).Take(5);

            var tasks = new List<Task>();
            foreach (var streamingLocator in links)
            {
                var t= client.StreamingLocators.DeleteAsync(_config.ResourceGroup, _config.AccountName,
                    streamingLocator.Name, cancellationToken: token);
                tasks.Add(t);
            }

            await Task.WhenAll(tasks);
        }


        public async Task<string> BuildUserStreamingLocatorAsync(long videoId, long userId, CancellationToken token)
        {
            var client = await _context;
            var locatorName = $"{videoId}-{userId}";
            var locator = await client.StreamingLocators.GetAsync(_config.ResourceGroup,
                _config.AccountName,
                locatorName,
                token);
            if (locator != null && locator.EndTime < DateTime.UtcNow)
            {
                await client.StreamingLocators.DeleteAsync(
                    _config.ResourceGroup,
                    _config.AccountName, locatorName, token);

                locator = null;
            }
            if (locator is null)
            {
                try
                {
                    await client.StreamingLocators.CreateAsync(
                        _config.ResourceGroup,
                        _config.AccountName,
                        locatorName,
                        new StreamingLocator
                        {
                            AssetName = $"video-{videoId}",
                            StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly,
                            EndTime = DateTime.UtcNow.AddHours(1)
                        }, token);


                }
                catch (ApiErrorException e) when (e.Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return null;
                }
                catch (ApiErrorException e) when (e.Response.StatusCode == HttpStatusCode.Conflict)
                {
                    await RemoveUnusedStreamingLocatorAsync(videoId, token);
                    return await BuildUserStreamingLocatorAsync(videoId, userId, token);
                }
            }
            return await GetStreamingUrlAsync(locatorName, token);
        }

        public async Task CreateShortStreamingLocator(long videoId, CancellationToken token)
        {
            var client = await _context;
            try
            {
                await client.StreamingLocators.CreateAsync(
                    _config.ResourceGroup,
                    _config.AccountName,
                    GetShortStreamingLocatorName(videoId),
                    new StreamingLocator
                    {
                        AssetName = $"video-short-{videoId}",
                        StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly
                    }, token);
            }
            catch (ApiErrorException ex) when (ex.Response.StatusCode == HttpStatusCode.BadRequest)
            {
                //Do nothing the locator exists
            }
        }

        private async Task<string> GetStreamingUrlAsync(string locatorName, CancellationToken token)
        {
            var client = await _context;
            //https://spitballdev-euno.streaming.media.azure.net/7a789e2a-f0a9-454b-a9fd-c6f9b2d55a93/file-f32a43d0-297d-44c4-94c2-daa.ism/manifest

            var streamingEndpoint = await client.StreamingEndpoints.GetAsync(_config.ResourceGroup, _config.AccountName, "default", cancellationToken: token);

            try
            {
                var paths = await client.StreamingLocators.ListPathsAsync(_config.ResourceGroup,
                    _config.AccountName, locatorName, token);
                var path = paths.StreamingPaths.Single(w => w.StreamingProtocol == StreamingPolicyStreamingProtocol.SmoothStreaming);
                if (path.Paths.Count > 0)
                {
                    var uriBuilder = new UriBuilder
                    {
                        Scheme = "https",
                        Host = streamingEndpoint.HostName,
                        Path = path.Paths[0]
                    };
                    return uriBuilder.ToString();
                }

            }
            catch (ApiErrorException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            return null;

        }

        [Cache(TimeConst.Hour, "media-service", true)]
        public Task<string> GetShortStreamingUrlAsync(long videoId, CancellationToken token)
        {
            return GetStreamingUrlAsync(GetShortStreamingLocatorName(videoId), token);
        }


        private static string GetShortStreamingLocatorName(long videoId)
        {
            return $"{videoId}-short-video";
        }


    }
}
