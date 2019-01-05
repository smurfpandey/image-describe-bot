using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportAPI
{
    public class MongoHelper
    {
        private static readonly string CONNECTION_STRING_NAME = "ImageDescBotDb";
        private static readonly string DATABASE_NAME = "imagedescbot";
        private static IMongoDatabase _database;

        public static IMongoDatabase GetDatabase(IConfiguration config)
        {
            if (_database != null)
            {
                return _database;
            }

            MongoClient _client = new MongoClient(config.GetConnectionString(CONNECTION_STRING_NAME));
            _database = _client.GetDatabase(DATABASE_NAME);

            return _database;
        }
    }
}
