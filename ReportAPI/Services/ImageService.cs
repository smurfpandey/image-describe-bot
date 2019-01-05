using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using ReportAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportAPI.Services
{
    public class ImageService
    {
        private const string COLLECTION_NAME = "images";

        private readonly IMongoCollection<Image> _images;

        public ImageService(IConfiguration config)
        {            
            _images = MongoHelper.GetDatabase(config).GetCollection<Image>(COLLECTION_NAME);
        }

        public Image Create(Image image)
        {
            _images.InsertOne(image);
            return image;
        }

        public Image Get(string id)
        {

            return _images.Find<Image>(image => image.Id == id).FirstOrDefault();
        }
    }
}
