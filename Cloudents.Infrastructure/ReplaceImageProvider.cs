using System;
using System.Collections.Generic;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public class ReplaceImageProvider : IReplaceImageProvider
    {
        private readonly IBlobProvider<SpitballContainer> _blobProvider;

        private readonly Dictionary<string,string> _sources = new Dictionary<string,string>(StringComparer.CurrentCultureIgnoreCase)
        {
            ["courseHero"] = "courseHero.png",
            ["Cram"] = "cram.png",
            ["Koofers"] = "koofers.png",
            ["Quizlet"] = "quizLet.png",
            ["Studyblue"] = "studyBlue.png",
            ["Studysoup"] = "studySoup.png",
            ["Spitball"] = "spitball.png",
            ["Cliffsnotes"] = "CliffsNotes.png",
            ["oneclass"] = "oneClass.png",
            ["chegg"] = "chegg.png",
            ["brainscape"] = "brainscape.png"
        };

        //TODO: make sure this is https - need to figure out how to solve this
        private static readonly SortedSet<string> InvalidImages = new SortedSet<string>(StringComparer.OrdinalIgnoreCase) {
            "https://www.coursehero.com/assets/img/coursehero_logo.png",
            "https://studysoup.com/assets/facebook-message-thumbnail-32320c1969a82f18331270299701b729.jpg",
            "https://classconnection.s3.amazonaws.com/images/fb-og-deck.png",
            "https://www.studyblue.com/css/images/webprintLogo.jpg",
            "https://oneclass.com/assets/home/og-home-new-2.jpg",
            "https://c.cheggcdn.com/assets/site/marketing/icons/icon-studenthub-200x200.png",
            "https://www.bing.com/assets/bsc-share-icon.png"
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

            if (InvalidImages.Contains(image.AbsoluteUri))
            {
                return _blobProvider.GetBlobUrl(val.ToLowerInvariant(), true);
            }

            return image;
        }
    }
}
