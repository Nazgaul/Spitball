using System;
using System.Linq;
using System.Collections.Generic;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure
{
    public class ReplaceImageProvider
    {
        private readonly IBlobProvider<SpitballContainer> _blobProvider;
        private readonly Dictionary<string,string> _sources = new Dictionary<string,string>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["courseHero.com"] = "coursehero.png",
            ["Cram.com"] = "cram.png",
            ["Koofers.com"] = "koofers.png",
            ["Quizlet.com"] = "quizlet.png",
            ["Studyblue.com"] = "studyblue.png",
            ["Studysoup.com"] = "studysoup.png"
        };


        private readonly List<string> _invalidImages = new List<string>
        {
            "https://www.coursehero.com/assets/img/coursehero_logo.png",
            "https://studysoup.com/assets/facebook-message-thumbnail-32320c1969a82f18331270299701b729.jpg",
            "https://classconnection.s3.amazonaws.com/images/fb-og-deck.png",
            "http://www.studyblue.com/css/images/webprintLogo.jpg"
        };


        public ReplaceImageProvider(IBlobProvider<SpitballContainer> blobProvider)
        {
            _blobProvider = blobProvider;
        }


        public Uri ChangeImageIfNeeded(string host, Uri image)
        {
            if (!_sources.TryGetValue(host, out var val)) return image;
            if (image == null)
            {
                return _blobProvider.GetBlobUrl(val.ToLowerInvariant(), true);
            }

            if (_invalidImages.Contains(image.AbsoluteUri, StringComparer.InvariantCultureIgnoreCase))
            {
                return _blobProvider.GetBlobUrl(val.ToLowerInvariant(), true);
            }

            return image;
        }
    }
}
