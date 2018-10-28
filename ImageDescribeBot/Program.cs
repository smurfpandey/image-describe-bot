using ImageDescribeBot.Model;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace ImageDescribeBot
{
    class Program
    {
        static string MS_API_KEY_NAME = "MICROSOFT_COMPUTER_VISION_API_KEY";
        static string MS_API_ENDPOINT_NAME = "MICROSOFT_COMPUTER_VISION_API_ENDPOINT";

        static void Main(string[] args)
        {
            // load environment variables from local file
            DotNetEnv.Env.Load();

            string msApiKey = Environment.GetEnvironmentVariable(MS_API_KEY_NAME);
            string msApiEndpoint = Environment.GetEnvironmentVariable(MS_API_ENDPOINT_NAME);

            Stopwatch timer;
            long timeSpentWikiGetImage;
            long timeSpentMSDescribeImage;

            HttpClient httpClient = new HttpClient();
            WikimediaHelper objWikiClient = new WikimediaHelper();
            AzureHelper objAzureClient = new AzureHelper(msApiKey, msApiEndpoint);

            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

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

            timer = Stopwatch.StartNew();
            string msSaysWhat = objAzureClient.DescribeImageUri(objImage.Url).Result;
            timer.Stop();

            timeSpentMSDescribeImage = timer.ElapsedMilliseconds;

            Console.WriteLine("MS said in {1}ms: {0}", msSaysWhat, timeSpentMSDescribeImage);

            Console.WriteLine("Hello World!");
        }
    }
}
