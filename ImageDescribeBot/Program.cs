using ImageDescribeBot.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

namespace ImageDescribeBot
{
    class Program
    {
        static string MS_API_ENABLED = "MICROSOFT_COMPUTER_VISION";
        static string MS_API_KEY_NAME = "MICROSOFT_COMPUTER_VISION_API_KEY";
        static string MS_API_ENDPOINT_NAME = "MICROSOFT_COMPUTER_VISION_API_ENDPOINT";

        static string GOOGLE_API_ENABLED = "GOOGLE_COMPUTER_VISION";

        static string AWS_API_ENABLED = "AWS_COMPUTER_VISION";
        static string AWS_ACCESS_KEY_NAME = "AWS_ACCESS_KEY";
        static string AWS_SECRET_KEY_NAME = "AWS_SECRET_KEY";

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

            Stopwatch timer;
            long timeSpentWikiGetImage;
            long timeSpentMSDescribeImage;
            long timeSpentGoogleLabelImage;
            long timeSpentAWSLabelImage;

            HttpClient httpClient = new HttpClient();
            WikimediaHelper objWikiClient = new WikimediaHelper();
            AzureHelper objAzureClient = null;
            GoogleHelper objGoogleClient = null;
            AWSHelper objAWSClient = null;

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

            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

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

            #region What did the MS say
            if (objAzureClient == null)
            {
                Console.WriteLine("MS API not enabled. Skipped.");
            }
            else
            {
                timer = Stopwatch.StartNew();
                string msSaysWhat = objAzureClient.DescribeImageUri(objImage.Url).Result;
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
                List<string> googleSaysWhat = objGoogleClient.LabelImageFromUri(objImage.Url).Result;
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
                List<string> awsSaysWhat = objAWSClient.DetectImageLabelFromUri(objImage.Url).Result;
                timer.Stop();

                timeSpentAWSLabelImage = timer.ElapsedMilliseconds;

                Console.WriteLine("AWS said in {1}ms: {0}", string.Join(',', awsSaysWhat), timeSpentAWSLabelImage);
            }

            #endregion

            #region TODO: Post to twitter

            #endregion

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
