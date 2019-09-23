using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Video;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Nito.AsyncEx;

namespace Cloudents.Infrastructure.Video
{
    public class MediaServices : IVideoService
    {
        public const string JobLabelImage = "image";
        public const string JobLabelShortVideo = "video-short";
        public const string JobLabelFullVideo = "video";




        private readonly AsyncLazy<AzureMediaServicesClient> _context;
        // private static CloudBlobClient _client;
        private ConfigWrapper _config;

        public const string PrefixThumbnailBlobName = "Thumbnail";

        public MediaServices(bool isDevelop)
        {

            _context = new AsyncLazy<AzureMediaServicesClient>(async () => await Init(isDevelop));
        }


        private const string PreviewTransformer = "PreviewTransformer";
        private const string StreamingTransformer = "StreamingTransformer";
        //public const string JobLabelShortVideo = "video";
        //private const string VideoThumbnailPrefix = "video-thumbnail-";
        //public const string VideoShortPrefix = "video-short-";

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

            //    var configuration = File.ReadAllText(jsonFile);
            //_config = JsonConvert.DeserializeObject<ConfigWrapper>(configuration);
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
                PreviewTransformer,
                new List<TransformOutput>()
                {
                    new TransformOutput(new BuiltInStandardEncoderPreset()
                    {
                        PresetName = EncoderNamedPreset.AdaptiveStreaming
                    }, OnErrorType.ContinueJob),
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

            await Task.WhenAll(t1, t2);
            return client;
            //TODO: need to add event to queue




        }


        public Task CreateVideoPreviewJobAsync(long id, string url, CancellationToken token)
        {
            // This example shows how to encode from any HTTPs source URL - a new feature of the v3 API.  
            // Change the URL to any accessible HTTPs URL or SAS URL from Azure.

            var t1 =  CreatePreviewJobAsync(id, url, token);
            var t2 =  CreateStreamingJobAsync(id, url, token);

            return Task.WhenAll(t1, t2);
            //return job;
        }



        public async Task DeleteImageAssetAsync(long id, CancellationToken token)
        {
            var v = await _context;

            var assetName = BuildAssetName(id, AssetType.Thumbnail);
            await v.Assets.DeleteAsync(_config.ResourceGroup, _config.AccountName, assetName, token);
        }




        private async Task CreatePreviewJobAsync(long id, string url, CancellationToken token)
        {
            var videoAsset = await CreateOutputAssetAsync(id, AssetType.Short, token);
            var thumbnailAsset = await CreateOutputAssetAsync(id, AssetType.Thumbnail, token);
            var jobInput =
                new JobInputHttp(files: new[] { url }, end: new AbsoluteClipTime(TimeSpan.FromSeconds(30)));

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(videoAsset.Name, label: JobLabelShortVideo),
                new JobOutputAsset(thumbnailAsset.Name, label: JobLabelImage)
            };

            var v = await _context;

            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, Get methods on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            await v.Jobs.CreateAsync(
                _config.ResourceGroup,
                _config.AccountName,
                PreviewTransformer,
                $"{id} {Guid.NewGuid():N}",
                new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs,
                }, token);
        }

        private async Task CreateStreamingJobAsync(long id, string url, CancellationToken token)
        {
            var videoAsset = await CreateOutputAssetAsync(id, AssetType.Long, token);
            var jobInput =
                new JobInputHttp(files: new[] { url });

            JobOutput[] jobOutputs =
            {
                new JobOutputAsset(videoAsset.Name, label: JobLabelFullVideo),
            };

            var v = await _context;

            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, Get methods on entities returns null 
            // if the entity doesn't exist (a case-insensitive check on the name).
            await v.Jobs.CreateAsync(
                _config.ResourceGroup,
                _config.AccountName,
                StreamingTransformer,
                $"{id} {Guid.NewGuid():N}",
                new Job
                {
                    Input = jobInput,
                    Outputs = jobOutputs,
                }, cancellationToken: token);
        }


        private async Task<Asset> CreateOutputAssetAsync(long id, AssetType assetType, CancellationToken token)
        {
            var assetName = BuildAssetName(id, assetType);
            var v = await _context;
            var asset = new Asset(name: assetName, description: assetName, container: assetName, alternateId: id.ToString());
            var outputAssetName = assetName;
            return await v.Assets.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName,
                outputAssetName, asset, token);
        }

        public async Task<string> GetAssetContainerAsync(long id, AssetType assetType, CancellationToken token)
        {
            var v = await _context;

            var assetName = BuildAssetName(id, assetType);
            var asset = await v.Assets.GetAsync(_config.ResourceGroup, _config.AccountName, assetName, token);
            
            return asset.Container;
        }

        private string BuildAssetName(long id, AssetType assetType)
        {
            return $"{assetType}{id}";
            
        }

      

        public async Task<string> BuildUserStreamingLocatorAsync(long videoId, CancellationToken token)
        {
            var client = await _context;
            var locatorName = $"{videoId} {Guid.NewGuid():N}";
            await client.StreamingLocators.CreateAsync(
                 _config.ResourceGroup,
                 _config.AccountName,
                 locatorName,
                 new StreamingLocator
                 {
                     AssetName = $"video-{videoId}",
                     StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly,
                     EndTime = DateTime.UtcNow.AddHours(1)
                 }, cancellationToken: token);

            return await GetStreamingUrlAsync(locatorName, token);
        }

        public  async Task CreateShortStreamingLocator(long videoId, CancellationToken token)
        {
            var client = await _context;
            await client.StreamingLocators.CreateAsync(
                _config.ResourceGroup,
                _config.AccountName,
                $"video-short-locator-{videoId}",
                new StreamingLocator
                {
                    AssetName = $"video-short-{videoId}",
                    StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly
                }, cancellationToken: token);
        }

        private async Task<string> GetStreamingUrlAsync(string locatorName, CancellationToken token)
        {
            var client = await _context;
            //https://spitballdev-euno.streaming.media.azure.net/7a789e2a-f0a9-454b-a9fd-c6f9b2d55a93/file-f32a43d0-297d-44c4-94c2-daa.ism/manifest

            var streamingEndpoint = await client.StreamingEndpoints.GetAsync(_config.ResourceGroup, _config.AccountName, "default", cancellationToken: token);
            var paths = await client.StreamingLocators.ListPathsAsync(_config.ResourceGroup,
                _config.AccountName, locatorName, token);
            var path = paths.StreamingPaths.Single(w => w.StreamingProtocol == StreamingPolicyStreamingProtocol.SmoothStreaming);
            var uriBuilder = new UriBuilder
            {
                Scheme = "https",
                Host = streamingEndpoint.HostName,
                Path = path.Paths[0]
            };
            return uriBuilder.ToString();
        }

        public Task<string> GetShortStreamingUrlAsync(long videoId, CancellationToken token)
        {
            return GetStreamingUrlAsync($"video-short-locator-{videoId}", token);
        }

      
    }
}
