﻿using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using ImageResizer.Plugins;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ImageResizer.Storage;
using ImageResizer.ExtensionMethods;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;

namespace Zbang.Cloudents.Images
{
    public class AzureReader3Plugin : BlobProviderBase, IMultiInstancePlugin 
    {
        public CloudBlobClient CloudBlobClient { get; set; }
        string blobStorageConnection;
        string blobStorageEndpoint;


        public bool RedirectToBlobIfUnmodified { get; set; }

        public AzureReader3Plugin()
            : base()
        {
            this.VirtualFilesystemPrefix = "~/azure";

        }
        public AzureReader3Plugin(NameValueCollection args):this() {
            LoadConfiguration(args);
            blobStorageConnection = args["connectionstring"];
            blobStorageEndpoint = args.GetAsString("blobstorageendpoint", args.GetAsString("endpoint",null));
            RedirectToBlobIfUnmodified = args.Get<bool>("redirectToBlobIfUnmodified", true);

        }


        protected Task<ICloudBlob> GetBlobRefAsync(string virtualPath)
        {
            string subPath = StripPrefix(virtualPath).Trim('/', '\\');

            string relativeBlobURL = string.Format("{0}/{1}", CloudBlobClient.BaseUri.OriginalString.TrimEnd('/', '\\'), subPath);

            return CloudBlobClient.GetBlobReferenceFromServerAsync(new Uri(relativeBlobURL));
        }
        public override async Task<IBlobMetadata> FetchMetadataAsync(string virtualPath, NameValueCollection queryString)
        {
            
            try
            {
                var cloudBlob = await GetBlobRefAsync(virtualPath);
                
                var meta = new BlobMetadata();
                meta.Exists = true; //Otherwise an exception would have happened at FetchAttributes
                var utc = cloudBlob.Properties.LastModified;
                if (utc != null)
                {
                    meta.LastModifiedDateUtc = utc.Value.UtcDateTime;
                }
                return meta;
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == 404)
                {
                    return new BlobMetadata() { Exists = false };
                }
                else
                {
                    throw;
                }
            }
        }

        public override async Task<Stream> OpenAsync(string virtualPath, NameValueCollection queryString)
        {

            MemoryStream ms = new MemoryStream(4096); // 4kb is a good starting point.

            // Synchronously download
            try
            {
                var cloudBlob = await GetBlobRefAsync(virtualPath); //TODO: Skip a round trip and skip getting the blob reference.
                await cloudBlob.DownloadToStreamAsync(ms);
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == 404)
                {
                    throw new FileNotFoundException("Azure blob file not found", e);
                }
                throw;
                
            }

            ms.Seek(0, SeekOrigin.Begin); // Reset to beginning
            return ms;
        }

        public override IPlugin Install(ImageResizer.Configuration.Config c) {
            if (string.IsNullOrEmpty(blobStorageConnection))
                throw new InvalidOperationException("AzureReader2 requires a named connection string or a connection string to be specified with the 'connectionString' attribute.");

            // Setup the connection to Windows Azure Storage
            //Lookup named connection string first, then fall back.
            var connectionString = CloudConfigurationManager.GetSetting(blobStorageConnection);
            if (string.IsNullOrEmpty(connectionString)) { connectionString = blobStorageConnection; }
            

            CloudStorageAccount cloudStorageAccount;
            if (CloudStorageAccount.TryParse(connectionString, out cloudStorageAccount)){
                if (string.IsNullOrEmpty(blobStorageEndpoint)){
                    blobStorageEndpoint = cloudStorageAccount.BlobEndpoint.ToString();
                }
            }else{
                throw new InvalidOperationException("Invalid AzureReader2 connectionString value; rejected by Azure SDK.");
            }
            if (!blobStorageEndpoint.EndsWith("/"))
                blobStorageEndpoint += "/";

            CloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            // Register rewrite
            c.Pipeline.PostRewrite += Pipeline_PostRewrite;

            base.Install(c);

            return this;
        }
        /// <summary>
        /// Removes the plugin from the given configuration container
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public override bool Uninstall(ImageResizer.Configuration.Config c)
        {
            c.Pipeline.PostRewrite -= Pipeline_PostRewrite;
            return base.Uninstall(c);
        }

        /// <summary>
        /// In case there is no querystring attached to the file (thus no operations on the fly) we can
        /// redirect directly to the blob. This let us offload traffic to blob storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="context"></param>
        /// <param name="e"></param>
        void Pipeline_PostRewrite(IHttpModule sender, HttpContext context, ImageResizer.Configuration.IUrlEventArgs e)
        {
            string prefix = VirtualFilesystemPrefix;

            // Check if prefix is within virtual file system and if there is no querystring
            if (RedirectToBlobIfUnmodified && Belongs(e.VirtualPath) && !c.Pipeline.HasPipelineDirective(e.QueryString)) {

                // Strip prefix from virtual path; keep container and blob
                string relativeBlobURL = e.VirtualPath.Substring(prefix.Length).TrimStart('/', '\\');

                // Redirect to blob
                //TODO: Add shared access signature if enabled
                context.Response.Redirect(blobStorageEndpoint + relativeBlobURL);
            }
        }
    }
}