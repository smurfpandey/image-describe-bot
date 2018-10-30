using Google.Cloud.Vision.V1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ImageDescribeBot
{
    class GoogleHelper
    {
        private ImageAnnotatorClient client;
        private double _confidenceThreshold;

        public GoogleHelper(double confidenceThreshold = 0.75)
        {
            client = ImageAnnotatorClient.Create();
            _confidenceThreshold = confidenceThreshold;
        }

        public async Task<List<string>> LabelImageFromUri(string imageUri)
        {
            List<string> lstLabels = new List<string>();
            try
            {
                Image googleImage = Image.FromUri(imageUri);
                IReadOnlyList<EntityAnnotation> labels = await client.DetectLabelsAsync(googleImage);

                foreach (EntityAnnotation label in labels)
                {
                    if (label.Score >= _confidenceThreshold)
                    {
                        lstLabels.Add(label.Description);
                    }
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
