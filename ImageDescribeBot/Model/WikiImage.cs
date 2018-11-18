using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ImageDescribeBot.Model
{
    class WikimediaResponse
    {
        [JsonProperty("pageid")]
        public int PageId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageinfo")]
        public List<WikiImage> ImageInfo = new List<WikiImage>();

        [JsonProperty("categories")]
        public List<dynamic> Category = new List<dynamic>();

        [JsonProperty("globalusage")]
        public List<dynamic> GlobalUsage = new List<dynamic>();
    }

    class WikiImage
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("extmetadata")]
        public ImageMetadata Metadata = new ImageMetadata();

        [JsonProperty("mediatype")]
        public string MediaType { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("descriptionurl")]
        public string DescriptionUrl { get; set; }
    }

    class ImageMetadata
    {
        public dynamic ObjectName { get; set; }

        public dynamic Restrictions { get; set; }

        public dynamic ImageDescription { get; set; }

        public dynamic Categories { get; set; }
    }
}
