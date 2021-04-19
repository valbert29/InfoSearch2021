using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Parsers.HtmlParser
{
    interface IParser
    {
        string GetText(string html);

        IEnumerable<string> GetLinks(string html, Uri uri);
    }
}
