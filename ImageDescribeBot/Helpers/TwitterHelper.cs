using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace ImageDescribeBot
{
    class TwitterHelper
    {
        public TwitterHelper(string consumerKey, string consumerSecret, string accessKey, string accessSecret)
        {
            // Set up
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessKey, accessSecret);
        }

        public void PostTweet(string imageDescUrl, byte[] imgData, string msCaption, string googleLabels, string awsLabels)
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
        }
    }
}
