using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Url { get; set; }
                        
        public string Title { get; set; }

        [JsonConverter(typeof(BsonDocJsonConverter))]
        public object WikimediaData { get; set; }
    }
}
