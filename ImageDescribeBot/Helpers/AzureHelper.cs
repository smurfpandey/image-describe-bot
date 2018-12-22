using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImageDescribeBot
{
    class AzureHelper
    {
        private ComputerVisionClient client;
        private static readonly ILogger _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AzureHelper(string subscriptionKey, string apiEndpoint)
        {
            ApiKeyServiceClientCredentials Credentials = new ApiKeyServiceClientCredentials(subscriptionKey);
            client = new ComputerVisionClient(Credentials);
            client.Endpoint = apiEndpoint;
        }

        public async Task<string> DescribeImage(byte[] imgBytes)
        {
            try
            {
                MemoryStream memStream = new MemoryStream(imgBytes);
                ImageDescription analysisResult = await client.DescribeImageInStreamAsync(memStream);                
                return analysisResult.Captions[0].Text;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting info from Azure", ex);
                return null;
            }

        }
    }
}
