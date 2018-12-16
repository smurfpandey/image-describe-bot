using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ImageDescribeBot
{
    class AWSHelper
    {
        private AmazonRekognitionClient client;
        private float _minConfidence;
        private int _maxLabels;

        public AWSHelper(string awsAccessKey, string awsSecretKey, float minConfidence = 75F, int maxLabels = 10)
        {
            client = new AmazonRekognitionClient(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USEast1);
            _minConfidence = minConfidence;
            _maxLabels = maxLabels;
        }

        public async Task<List<string>> LabelImage(byte[] imgBytes)
        {
            List<string> lstLabels = new List<string>();
            try
            {
                MemoryStream memStream = new MemoryStream(imgBytes);
                DetectLabelsRequest detectlabelsRequest = new DetectLabelsRequest()
                {
                    Image = new Image { Bytes = memStream },
                    MaxLabels = _maxLabels,
                    MinConfidence = _minConfidence
                };

                DetectLabelsResponse response = await client.DetectLabelsAsync(detectlabelsRequest);
                foreach (Label label in response.Labels)
                {
                    lstLabels.Add(label.Name);
                }                    
            }
            catch (Exception ex)
            {
                lstLabels.Add("Error");
            }

            return lstLabels;
        }
    }
}
