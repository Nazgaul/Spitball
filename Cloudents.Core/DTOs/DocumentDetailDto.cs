using System;
using System.IO;

namespace Cloudents.Core.DTOs
{
    public class DocumentDetailDto
    {
        private string _name;

        public long Id{ get; set; }

        public string Name
        {
            get => Path.GetFileNameWithoutExtension(_name);
            set => _name = value;
        }

        public DateTime Date { get; set; }

        public string University { get; set; }

        public string Course { get; set; }

        public string Professor { get; set; }

        public UserDto User { get; set; }

        public string Type { get; set; }

        public int Pages { get; set; }

        public int Views { get; set; }

        public int Downloads { get; set; }

        public decimal? Price { get; set; }

        public bool IsPurchased { get; set; }

        public int PageCount { get; set; }

    }

    public class ImageProperties
    {
        public long Id { get; set; }
        public int Page { get; set; }
        public bool Blur { get; set; }


        public static string ImageHashKey = "59b514174bffe4ae402b3d63aad79fe0";
    }
}
