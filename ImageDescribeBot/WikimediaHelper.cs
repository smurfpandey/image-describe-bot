using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImageDescribeBot.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDescribeBot
{
    class WikimediaHelper
    {
        private const string API_BASE = "https://commons.wikimedia.org/w/api.php";
        private static readonly string[] FORMATS = { ".png", ".jpg", ".jpeg", ".gif" };

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

            // make api rquest to wikimedia
            string response = await client.GetStringAsync(reqUri);

            JObject jObj = JsonConvert.DeserializeObject<JObject>(response);
            JToken imageData = jObj["query"]["pages"].First.First;

            WikimediaResponse objWImage = imageData.ToObject<WikimediaResponse>();

            if(IsValidImage(objWImage))
                return objWImage.ImageInfo[0];

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


            return true;
        }

    }
}
