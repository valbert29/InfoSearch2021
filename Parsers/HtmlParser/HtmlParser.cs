using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Parsers.HtmlParser
{
    public class HtmlParser : IParser
    {
        /// <summary>
        /// Получение ссылок с html страницы
        /// </summary>
        public IEnumerable<string> GetLinks(string html, Uri uri)
        {

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var hrefList = doc.DocumentNode.SelectNodes("//body//a")
                .Select(p => p.GetAttributeValue("href", ""))
                .Select(x => x = CorrectUrl(x, uri))
                .Where(x => x != null)
                .Distinct()
                .ToList();

            return hrefList;
        }

        /// <summary>
        /// Получение текста из html
        /// </summary>
        public string GetText(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.InnerText.Trim();
        }

        /// <summary>
        /// Приведение ссылки к корректному виду
        /// </summary>
        private string CorrectUrl(string url, Uri uri)
        {
            if (string.IsNullOrEmpty(url) || url.Contains(".jpg") || url.Contains(".png") || url.Contains(".jpeg"))
                return null;

            if (url.StartsWith("//"))
                url = $"{uri.Scheme}:{url}";

            if (url.StartsWith("/"))
                url = $"{uri.Scheme}://{uri.Host}{url}";

            if (!url.StartsWith("http") && !url.Contains(uri.Host))
                url = $"{uri.Scheme}://{uri.Host}/{url}";

            if (url.Contains('#'))
                url = url.Substring(0, url.IndexOf('#'));

            if (url.EndsWith("//"))
                url = url.Remove(url.Length - 2, 2);

            if (url.EndsWith("/"))
                url = url.Remove(url.Length - 1, 1);

            if (!url.Contains(uri.Host))
                return null;

            return url;
        }
    }
}
