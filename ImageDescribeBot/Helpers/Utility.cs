﻿
using AngleSharp.Parser.Html;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ImageDescribeBot
{
    class Utility
    {
        private static readonly ILogger _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static string GetTextFromHtml(string htmlText)
        {
            var dom = new HtmlParser().Parse(htmlText);
            return dom.Body.TextContent;
        }

        public static async Task<byte[]> DownloadImage(string imageUri)
        {
            try
            {
                HttpClient client = new HttpClient();

                var productValue = new ProductInfoHeaderValue("ImageDescBot", "1.0");
                var commentValue = new ProductInfoHeaderValue("(+https://github.com/smurfpandey/ImageDescribeBot)");

                client.DefaultRequestHeaders.UserAgent.Add(productValue);
                client.DefaultRequestHeaders.UserAgent.Add(commentValue);
                MemoryStream memStream = new MemoryStream();

                Stream stream = await client.GetStreamAsync(imageUri);
                await stream.CopyToAsync(memStream);

                return memStream.ToArray();
            }
            catch(Exception ex)
            {
                _logger.Error("error download image", ex);
                return null;
            }
            
        }
    }
}
