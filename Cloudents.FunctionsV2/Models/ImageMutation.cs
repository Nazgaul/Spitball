using System;
using Cloudents.Core.DTOs;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Processing;

namespace Cloudents.FunctionsV2.Models
{
    public class ImageMutation
    {
        public static ImageMutation FromQueryString(IQueryCollection query)
        {
            int.TryParse(query["round"], out var round);
            Enum.TryParse(query["anchorPosition"], true, out AnchorPositionMode position);

            var imageMutation = new ImageMutation(position, round);
            if (int.TryParse(query["version"], out var version))
            {
                imageMutation.Version = version;
            }
            if (int.TryParse(query["width"], out var width) &&
                int.TryParse(query["height"], out var height))
            {
                if (!Enum.TryParse(query["mode"], true, out ResizeMode mode))
                {
                    mode = ResizeMode.Crop;
                }
                imageMutation.Resize = (mode,width,height);
            }

            if (!string.IsNullOrEmpty(query["background"].ToString()))
            {
                imageMutation.Background = query["background"];
            }

            return imageMutation;
        }

        public string CacheString()
        {
            return
                $"{RoundCorner}_{BlurEffect}_{Position}_{Background}_{Version}_{Resize?.Mode}_{Resize?.Height}_{Resize?.Width}";
        }

        public override string ToString()
        {
            return $"{nameof(CenterCords)}: {CenterCords}," +
                   $" {nameof(RoundCorner)}: {RoundCorner}," +
                   $" {nameof(Resize)}: {Resize}," +
                   $" {nameof(BlurEffect)}: {BlurEffect}," +
                   $" {nameof(Position)}: {Position}," +
                   $" {nameof(Background)}: {Background}";
        }

        private ImageMutation(
            AnchorPositionMode position, int roundCorner)
        {
            Position = position;
            RoundCorner = roundCorner;
        }

        public (float x,float y)? CenterCords { get; set; }

        public int RoundCorner { get; }

        public (ResizeMode Mode, int Width, int Height)? Resize { get; private set; }

        public ImageProperties.BlurEffect BlurEffect { get; set; }
        public AnchorPositionMode Position { get; }
        public string? Background { get; set; }

        public int Version { get; private set; }
    }
}