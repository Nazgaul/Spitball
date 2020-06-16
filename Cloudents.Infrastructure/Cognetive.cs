﻿using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Cloudents.Infrastructure
{


    public class CognitiveService : ICognitiveService
    {
        private readonly IFaceClient _faceClient;
        public CognitiveService()
        {
            _faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials("89af392451dc4be587fbfa6273a3d65a"))
            {
                Endpoint = "https://sb-cognitive-face.cognitiveservices.azure.com",

            };
        }



        /* 
 * DETECT FACES
 * Detects features from faces and IDs them.
 */
        public async Task<Point?> DetectCenterFaceAsync(Stream stream, CancellationToken token)
        {
            try
            {
                var result = await _faceClient.Face.DetectWithStreamAsync(stream, cancellationToken: token);

                var faceRectangle = result.FirstOrDefault()?.FaceRectangle;
                if (faceRectangle is null)
                {
                    return null;
                }

                var left = faceRectangle.Left + (faceRectangle.Width / 2);
                var top = faceRectangle.Top + faceRectangle.Height / 2;
                return new Point(left, top);
            }
            catch (APIErrorException e) when (e.Response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (e.Body.Error.Code == "InvalidImage" && e.Body.Error.Message == "Decoding error, image format unsupported.")
                {
                    return null;
                }
                if (e.Body.Error.Code == "InvalidImageSize")
                {
                    return null;
                }
                throw;
            }


        }
    }
}
