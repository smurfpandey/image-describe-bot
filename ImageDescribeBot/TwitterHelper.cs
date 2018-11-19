using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;

namespace ImageDescribeBot
{
    class TwitterHelper
    {
        public TwitterHelper(string consumerKey, string consumerSecret, string accessKey, string accessSecret)
        {
            // Set up
            Auth.SetUserCredentials(consumerKey, consumerSecret, accessKey, accessSecret);
        }

        public void PostTweet(string imageUrl, string msCaption, string googleLabels, string awsLabels)
        {
            string tweetTemplate = "Microsoft: {0}\r\n"
                + "Google: {1}\r\n"
                + "AWS: {2}\r\n"
                + "\r\n"
                + "{3}";

            string tweetText = string.Format(tweetTemplate, msCaption, googleLabels, awsLabels, imageUrl);

            var tweet = Tweet.PublishTweet(tweetText);
        }
    }
}
