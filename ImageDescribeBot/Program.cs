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
        static string GOOGLE_API_KEY = "GOOGLE_COMPUTER_VISION_API_KEY";

        static void Main(string[] args)
        {
            // load environment variables from local file
            DotNetEnv.Env.Load();

            #region Initialize stuff

            bool msApiEnabled = Convert.ToBoolean(Environment.GetEnvironmentVariable(MS_API_ENABLED));
            string msApiKey = Environment.GetEnvironmentVariable(MS_API_KEY_NAME);
            string msApiEndpoint = Environment.GetEnvironmentVariable(MS_API_ENDPOINT_NAME);

            bool googleApiEnabled = Convert.ToBoolean(Environment.GetEnvironmentVariable(GOOGLE_API_ENABLED));            

            Stopwatch timer;
            long timeSpentWikiGetImage;
            long timeSpentMSDescribeImage;
            long timeSpentGoogleLabelImage;
            long timeSpentAWSLabelImage;

            HttpClient httpClient = new HttpClient();
            WikimediaHelper objWikiClient = new WikimediaHelper();
            AzureHelper objAzureClient = null;
            GoogleHelper objGoogleClient = null;

            if (msApiEnabled)
            {
                objAzureClient = new AzureHelper(msApiKey, msApiEndpoint);
            }

            if (googleApiEnabled)
            {
                objGoogleClient = new GoogleHelper();
            }

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

            #region TODO: What did the Google say

            if(objGoogleClient == null)
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

            #region TODO: What did the AWS say

            #endregion

            #region TODO: Post to twitter

            #endregion

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
