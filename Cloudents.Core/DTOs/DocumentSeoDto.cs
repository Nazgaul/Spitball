﻿namespace Cloudents.Core.DTOs
{
    public class DocumentSeoDto
    {
        public string Name { get; set; }
        public string BoxName { get; set; }
        public string Description { get; set; }

        public string Discriminator { get; set; }

        public string ImageUrl { get; set; }

    }
}
