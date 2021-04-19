using HtmlAgilityPack;
using System.Collections.Generic;

namespace Crawler
{
    class ParserService
    {
        public static HtmlDocument DownloadDocument(string url)
        {
            HtmlDocument doc;
            var webGet = new HtmlWeb();
            doc = webGet.Load(url);
            return doc;
        }

        public static List<string> GetPageLinks(HtmlDocument doc, string currentPageUrl)
        {
            HtmlNode parent = doc.GetElementbyId("post-content-body");
            HtmlNodeCollection childs = parent.ChildNodes;
            var hrefList = new List<string>();
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute hrefAtt = link.Attributes["href"];
                if (hrefAtt.Value.StartsWith("http"))
                    hrefList.Add(hrefAtt.Value);
            }
            return hrefList;
        }

        //public static 
    }
}
