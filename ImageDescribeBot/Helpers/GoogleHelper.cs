using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImageDescribeBot
{
    class GoogleHelper
    {
        private static readonly ILogger _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ImageAnnotatorClient client;
        private double _confidenceThreshold;

        public GoogleHelper(double confidenceThreshold = 0.75)
        {
            client = ImageAnnotatorClient.Create();
            _confidenceThreshold = confidenceThreshold;
        }

        public async Task<List<string>> LabelImage(byte[] imgBytes)
        {
            try
            {
                List<string> lstLabels = new List<string>();
                Image googleImage = Image.FromBytes(imgBytes);
                IReadOnlyList<EntityAnnotation> labels = await client.DetectLabelsAsync(googleImage);

                foreach (EntityAnnotation label in labels)
                {
                    if (label.Score >= _confidenceThreshold)
                    {
                        lstLabels.Add(label.Description);
                    }
                }
                return lstLabels;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting info from Google", ex);
                return null;
            }

        }
    }
}
