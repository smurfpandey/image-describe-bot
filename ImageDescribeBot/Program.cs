using ImageDescribeBot.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace ImageDescribeBot
{
    class Program
    {
        static readonly string MS_API_ENABLED = "MICROSOFT_COMPUTER_VISION";
        static readonly string MS_API_KEY_NAME = "MICROSOFT_COMPUTER_VISION_API_KEY";
        static readonly string MS_API_ENDPOINT_NAME = "MICROSOFT_COMPUTER_VISION_API_ENDPOINT";

        static readonly string GOOGLE_API_ENABLED = "GOOGLE_COMPUTER_VISION";

        static readonly string AWS_API_ENABLED = "AWS_COMPUTER_VISION";
        static readonly string AWS_ACCESS_KEY_NAME = "AWS_ACCESS_KEY";
        static readonly string AWS_SECRET_KEY_NAME = "AWS_SECRET_KEY";

        static readonly string TWITTER_CONSUMER_KEY = "TWITTER_CONSUMER_KEY";
        static readonly string TWITTER_CONSUMER_SECRET = "TWITTER_CONSUMER_SECRET";
        static readonly string TWITTER_ACCESS_TOKEN = "TWITTER_ACCESS_TOKEN";
        static readonly string TWITTER_ACCESS_TOKEN_SECRET = "TWITTER_ACCESS_TOKEN_SECRET";


        static void Main(string[] args)
        {
            // load environment variables from local file
            DotNetEnv.Env.Load();

            #region Initialize stuff

            bool msApiEnabled = Convert.ToBoolean(Environment.GetEnvironmentVariable(MS_API_ENABLED));
            string msApiKey = Environment.GetEnvironmentVariable(MS_API_KEY_NAME);
            string msApiEndpoint = Environment.GetEnvironmentVariable(MS_API_ENDPOINT_NAME);

            bool googleApiEnabled = Convert.ToBoolean(Environment.GetEnvironmentVariable(GOOGLE_API_ENABLED));

            bool awsApiEnabled = Convert.ToBoolean(Environment.GetEnvironmentVariable(AWS_API_ENABLED));
            string awsAccessKey = Environment.GetEnvironmentVariable(AWS_ACCESS_KEY_NAME);
            string awsSecretKey = Environment.GetEnvironmentVariable(AWS_SECRET_KEY_NAME);

            string twitterConsumerKey = Environment.GetEnvironmentVariable(TWITTER_CONSUMER_KEY);
            string twitterConsumerSecretKey = Environment.GetEnvironmentVariable(TWITTER_CONSUMER_SECRET);
            string twitterAccessKey = Environment.GetEnvironmentVariable(TWITTER_ACCESS_TOKEN);
            string twitterAccessSecretKey = Environment.GetEnvironmentVariable(TWITTER_ACCESS_TOKEN_SECRET);

            Stopwatch timer;
            long timeSpentWikiGetImage;
            long timeSpentMSDescribeImage;
            long timeSpentGoogleLabelImage;
            long timeSpentAWSLabelImage;

            HttpClient httpClient = new HttpClient();
            Censorboard objCensor = new Censorboard();
            WikimediaHelper objWikiClient = new WikimediaHelper();
            AzureHelper objAzureClient = null;
            GoogleHelper objGoogleClient = null;
            AWSHelper objAWSClient = null;
            TwitterHelper objTwitter = null;

            string msSaysWhat = string.Empty;
            List<string> googleSaysWhat = new List<string>();
            List<string> awsSaysWhat = new List<string>();

            string imgUrl = string.Empty;
            byte[] imgBytes = null;

            if (msApiEnabled)
            {
                objAzureClient = new AzureHelper(msApiKey, msApiEndpoint);
            }

            if (googleApiEnabled)
            {
                objGoogleClient = new GoogleHelper();
            }

            if (awsApiEnabled)
                objAWSClient = new AWSHelper(awsAccessKey, awsSecretKey);

            objTwitter = new TwitterHelper(twitterConsumerKey, twitterConsumerSecretKey, twitterAccessKey, twitterAccessSecretKey);

            #endregion

            #region Get Image from Wikimedia

            Console.WriteLine("START: objWikiClient.GetImage()");
            timer = Stopwatch.StartNew();
            WikiImage objImage = objWikiClient.GetImage(httpClient).Result;
            timer.Stop();

            timeSpentWikiGetImage = timer.ElapsedMilliseconds;
            Console.WriteLine("END: objWikiClient.GetImage()");

            if (objImage == null)
            {
                Console.WriteLine("No image available.");
                return;
            }

            Console.WriteLine("Got image from Wiki in {1}ms: {0}", objImage.Url, timeSpentWikiGetImage);

            #endregion

            #region Download the image

            imgUrl = objImage.Url;
            if (objImage.Size > 3000000 || objImage.Width > 8192 || objImage.Height > 8192) {
                imgUrl = objImage.ThumbUrl;
            }

            imgBytes = Utility.DownloadImage(imgUrl).Result;

            #endregion

            #region What did the MS say
            if (objAzureClient == null)
            {
                Console.WriteLine("MS API not enabled. Skipped.");
            }
            else
            {
                timer = Stopwatch.StartNew();
                msSaysWhat = objAzureClient.DescribeImage(imgBytes).Result;
                timer.Stop();

                timeSpentMSDescribeImage = timer.ElapsedMilliseconds;

                Console.WriteLine("MS said in {1}ms: {0}", msSaysWhat, timeSpentMSDescribeImage);
            }
            #endregion

            #region What did the Google say
            if (objGoogleClient == null)
            {
                Console.WriteLine("Google API not enabled. Skipped.");
            }
            else
            {
                timer = Stopwatch.StartNew();
                googleSaysWhat = objGoogleClient.LabelImage(imgBytes).Result;
                timer.Stop();

                timeSpentGoogleLabelImage = timer.ElapsedMilliseconds;

                Console.WriteLine("Google said in {1}ms: {0}", string.Join(',', googleSaysWhat), timeSpentGoogleLabelImage);
            }
            #endregion

            #region What did the AWS say

            if (objAWSClient == null)
            {
                Console.WriteLine("AWS API not enabled. Skipped.");
            }
            else
            {
                timer = Stopwatch.StartNew();
                awsSaysWhat = objAWSClient.LabelImage(imgBytes).Result;
                timer.Stop();

                timeSpentAWSLabelImage = timer.ElapsedMilliseconds;

                Console.WriteLine("AWS said in {1}ms: {0}", string.Join(',', awsSaysWhat), timeSpentAWSLabelImage);
            }

            #endregion

            #region Post to twitter

            objTwitter.PostTweet(objImage.DescriptionUrl, imgBytes, msSaysWhat, string.Join(", ", googleSaysWhat), string.Join(", ", awsSaysWhat));

            #endregion

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
