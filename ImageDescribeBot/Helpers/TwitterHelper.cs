using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace ImageDescribeBot
{
    class TwitterHelper
    {
        private static readonly ILogger _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TwitterHelper(string consumerKey, string consumerSecret, string accessKey, string accessSecret)
        {
            // Set up
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessKey, accessSecret);
        }

        public string PostTweet(string imageDescUrl, byte[] imgData, string msCaption, string googleLabels, string awsLabels)
        {
            try
            {
                string tweetTemplate = "Microsoft: {0}\r\n"
                + "Google: {1}\r\n"
                + "AWS: {2}\r\n"
                + "\r\n"
                + "{3}";

                string tweetText = string.Format(tweetTemplate, msCaption, googleLabels, awsLabels, imageDescUrl);

                var media = Upload.UploadBinary(imgData);
                var tweet = Tweet.PublishTweet(tweetText, new PublishTweetOptionalParameters
                {
                    Medias = new List<IMedia> { media }
                });

                return tweet.Url;
            }
            catch(Exception ex)
            {
                _logger.Error("error posting to twitter", ex);
                return string.Empty;
            }
            
        }
    }
}
