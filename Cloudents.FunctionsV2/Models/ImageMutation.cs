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
            if (int.TryParse(query["width"], out var width) &&
                int.TryParse(query["height"], out var height))
            {
                if (!Enum.TryParse(query["mode"], true, out ResizeMode mode))
                {
                    mode = ResizeMode.Crop;
                }
                imageMutation.Resize = (mode,width,height);
            }

            return imageMutation;

            //if (width == 0)
            //{
            //    width = 50;
            //}

            //if (height == 0)
            //{
            //    height = 50;
            //}

            ////var centerCords = query["center"].ToArray()?.Select(s => float.Parse(s));

            //return new ImageMutation( position, round)
            //{
            //    Resize = 
            //};
        }

        private ImageMutation(
            // int width, int height, 
            // ResizeMode mode,
            AnchorPositionMode position, int roundCorner)
        {
            //Width = width;
            //Height = height;
            //Mode = mode;
            Position = position;
            RoundCorner = roundCorner;
            //CenterCords = centerCords ?? Array.Empty<float>();

        }

        public (float x,float y)? CenterCords { get; set; }
        //  public int Width { get; }
        // public int Height { get; }

        public int RoundCorner { get; }

        // public ResizeMode Mode { get; }

        public (ResizeMode Mode, int Width, int Height)? Resize { get; private set; }

        public ImageProperties.BlurEffect BlurEffect { get; set; }
        public AnchorPositionMode Position { get; }
        public string? Background { get; set; }
    }
}