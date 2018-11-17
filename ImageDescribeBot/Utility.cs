
using AngleSharp.Parser.Html;

namespace ImageDescribeBot
{
    class Utility
    {
        public static string GetTextFromHtml(string htmlText)
        {
            var dom = new HtmlParser().Parse(htmlText);
            return dom.Body.TextContent;
        }
    }
}
