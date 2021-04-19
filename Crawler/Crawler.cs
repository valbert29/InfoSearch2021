using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Crawler.Parsers.HtmlParser;

namespace Crawler.Crawler
{
    public class Crawler
    {
        private readonly HtmlParser _htmlParser;
        private readonly HttpClient _httpClient;
        private string ParentFolderPath { get; }
        private int PageCount { get; }
        private int WordCount { get; }

        public Crawler(string parentFolderPath, int pageCount = 100, int wordCount = 1000)
        {
            ParentFolderPath = parentFolderPath;
            PageCount = pageCount;
            WordCount = wordCount;
            CreateForder();
            _htmlParser = new HtmlParser();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
        }

        public void Crawl(string link)
        {
            var uri = new Uri(link);

            var links = new List<string>() { link };//текущие ссылки
            var indexed = new List<IndexLink>();//проверенные ссылки
            var newLinks = new List<string>(); //новые ссылки 
            var indexedBad = new List<string>();//494 или меньше 1000 слов
            var inQueue = new List<string>();//ссылки в очереди на запись

            while (indexed.Count < PageCount)
            {
                if (newLinks.Count > 0 || inQueue.Count > 0)
                {
                    links.AddRange(newLinks.Distinct());
                    links.AddRange(inQueue);
                    links.RemoveAll(x => indexed.Select(y => y.Link).Contains(x));
                    links.RemoveAll(x => indexedBad.Contains(x));
                    inQueue = links.Skip(PageCount - indexed.Count).Distinct().ToList();
                    links = links.Take(PageCount - indexed.Count).ToList();
                    newLinks = new List<string>();
                }

                if (links.Count < 1)
                {
                    Console.WriteLine("not enough links");
                    break;
                }

                //проходим по текущим ссылкам, новые добавляем в список и проверяем на кол-во слов
                foreach (var url in links)
                {
                    try
                    {
                        var html = _httpClient.GetStringAsync(url).Result;
                        newLinks.AddRange(_htmlParser.GetLinks(html, uri));
                        var text = _htmlParser.GetText(html);
                        var wordCount = GetWordsCount(text);
                        if (wordCount > WordCount)
                        {
                            indexed.Add(new IndexLink
                            {
                                Link = url,
                                WordCount = wordCount,
                                Text = text
                            });
                        }
                        else
                        {
                            indexedBad.Add(url);
                        }
                    }
                    catch (Exception ex)
                    {
                        indexedBad.Add(url);
                        continue;
                    }

                }
                links = new List<string>();
            }
            //сохраняем в файлы
            SaveToFile(indexed);

        }

        /// <summary>
        /// Сохранение в файл
        /// </summary>
        private void SaveToFile(List<IndexLink> indexLinks)
        {

            var uri = new Uri(indexLinks[0].Link);

            var pathToDir = $"{ParentFolderPath}/{uri.Host}";

            //Create forder for link
            if (!Directory.Exists(pathToDir))
            {
                Directory.CreateDirectory(pathToDir);
            }

            //save text in file
            for (int i = 0; i < indexLinks.Count; i++)
            {
                var fileName = $"{pathToDir}/{i}.txt";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                using (FileStream fs = File.Create(fileName))
                {
                    Byte[] title = new UTF8Encoding(true).GetBytes(indexLinks[i].Text);
                    fs.Write(title, 0, title.Length);
                    Console.WriteLine(fileName);
                }
                indexLinks[i].FilePath = fileName;
                indexLinks[i].NumDoc = i;
            }

            var indexFile = $"{pathToDir}/index.txt";
            if (File.Exists(indexFile))
            {
                File.Delete(indexFile);
            }
            using (FileStream fs = File.Create(indexFile))
            {
                string text = string.Join('\n', indexLinks.Select(x => $"{x.NumDoc} {x.WordCount} {x.FilePath} {x.Link}"));
                Byte[] title = new UTF8Encoding(true).GetBytes(text);
                fs.Write(title, 0, title.Length);
            }

        }

        private void CreateForder()
        {
            //Create forder for link
            if (!Directory.Exists(ParentFolderPath))
            {
                Directory.CreateDirectory(ParentFolderPath);
            }
        }

        private int GetWordsCount(string str)
        {
            MatchCollection collection = Regex.Matches(str, @"[\S]{3,}");
            return collection.Count;
        }

        private class IndexLink
        {
            public string Link { get; set; }

            public string Text { get; set; }

            public int WordCount { get; set; }

            public string FilePath { get; set; }

            public int NumDoc { get; set; }

        }
    }
}
