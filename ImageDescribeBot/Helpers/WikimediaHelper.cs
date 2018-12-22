using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImageDescribeBot.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace ImageDescribeBot
{
    class WikimediaHelper
    {
        private const string API_BASE = "https://commons.wikimedia.org/w/api.php";
        private static readonly string[] FORMATS = { ".png", ".jpg", ".jpeg", ".gif" };

        private Censorboard objCensor = new Censorboard();
        private static readonly ILogger _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<WikiImage> GetImage(HttpClient client)
        {
            string reqUri = API_BASE +
                "?action=query" +
                "&prop=imageinfo|categories|globalusage" +
                "&iiprop=url|size|extmetadata|mediatype" +
                "&iiurlheight=1080" +
                "&format=json" +
                "&generator=random" +
                "&grnnamespace=6";

            try
            {
                // make api rquest to wikimedia
                string response = await client.GetStringAsync(reqUri);

                JObject jObj = JsonConvert.DeserializeObject<JObject>(response);
                JToken imageData = jObj["query"]["pages"].First.First;

                WikimediaResponse objWImage = imageData.ToObject<WikimediaResponse>();

                if (IsValidImage(objWImage))
                    return objWImage.ImageInfo[0];
            }
            catch (Exception ex)
            {
                _logger.Error("error getting image from wikipedia", ex);
            }

            return null;
        }

        private bool IsValidImage(WikimediaResponse objWImage)
        {
            WikiImage objIInfo = objWImage.ImageInfo[0];

            // check that the file is actually a picture
            if (objIInfo.MediaType != "BITMAP")
                return false;

            // Make sure the picture is big enough
            if (objIInfo.Width <= 50 || objIInfo.Height <= 50)
                return false;

            // Make sure the format is supported
            string thisExtn = Path.GetExtension(objIInfo.Url);
            if (Array.IndexOf(FORMATS, thisExtn) < 0)
                return false;

            // We got a picture, now let's verify we can use it.
            
            //Check file name for bad words
            if (objCensor.IsBlacklisted(objWImage.Title))
            {
                Console.WriteLine("Image discarded, {0}. badword in page title: {1}", objIInfo.DescriptionUrl, objWImage.Title);
                return false;
            }

            // Check picture title for bad words
            if (objCensor.IsBlacklisted(objIInfo.Metadata.ObjectName.value.ToString()))
            {
                Console.WriteLine("Image discarded, {0}. badword in picture title: {1}", objIInfo.DescriptionUrl, objIInfo.Metadata.ObjectName.value.ToString());
                return false;
            }

            // Check restrictions for more bad words
            if (objCensor.IsBlacklisted(objIInfo.Metadata.Restrictions.value.ToString()))
            {
                Console.WriteLine("Image discarded, {0}. badword in restrictions: {1}", objIInfo.DescriptionUrl, objIInfo.Metadata.Restrictions.value.ToString());
                return false;
            }

            // Check file description for bad words
            if (objIInfo.Metadata.ImageDescription != null)
            {
                string cleanedDescription = Utility.GetTextFromHtml(objIInfo.Metadata.ImageDescription.value.ToString());
                if (objCensor.IsBlacklisted(cleanedDescription))
                {
                    Console.WriteLine("Image discarded, {0}. badword in image description: {1}", objIInfo.DescriptionUrl, cleanedDescription);
                    return false;
                }

                if (objCensor.ShouldFilterForPhrase(cleanedDescription))
                {
                    Console.WriteLine("Image discarded, {0}. blacklisted phrase in image description: {1}", objIInfo.DescriptionUrl, cleanedDescription);
                    return false;
                }
            }

            // The mediawiki API is awful, there's another list of categories which
            // is not the same as the one requested by asking for "categories".
            // Fortunately it's still in the API response, under extmetadata.
            List<string> lstCateg = new List<string>();
            lstCateg.Add(objIInfo.Metadata.Categories.value.ToString());
            foreach(dynamic objCateg in objWImage.Category)
            {
                lstCateg.Add(objCateg.title.ToString());
            }

            if (objCensor.ShouldFilterForCategory(lstCateg))
            {
                Console.WriteLine("Image discarded, {0}. blacklisted phrase in categories", objIInfo.DescriptionUrl);
                return false;
            }

            // #TODO:: check parent categories for each category in metadata,
            // and compare them against the blacklist too. This will require
            // extra API calls

            // if the picture is used in any wikipage with unwanted themes, we probably
            // don't want to use it.
            foreach (dynamic wikipage in objWImage.GlobalUsage)
            {
                if (objCensor.IsBlacklisted(wikipage.title.ToString()))
                {
                    Console.WriteLine("Image discarded, {0}. page usage {1}", objIInfo.DescriptionUrl, wikipage.title.ToString());
                    return false;
                }
                if (objCensor.ShouldFilterForCategory(wikipage.title.ToString()))
                {
                    Console.WriteLine("Image discarded, {0}. page usage {1}", objIInfo.DescriptionUrl, wikipage.title.ToString());
                    return false;
                }                
            }
            return true;
        }
    }
}
