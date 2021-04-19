using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Crawler.Task_2
{
    /// <summary>
    /// Стеммер Портера
    /// </summary>
    public class Stemming
    {
        private string ParentForderPath { get; }
        private readonly Uri uri;
        private readonly FileProvider fileProvider;

        public Stemming(string domain, string parentForderPath)
        {
            ParentForderPath = parentForderPath;
            uri = new Uri(domain);
            fileProvider = new FileProvider();
            var stemmingforder = $"{ParentForderPath}/{uri.Host}/stemming/";
            if (!Directory.Exists(stemmingforder))
            {//Create forder for link
                Directory.CreateDirectory(stemmingforder);
            }

        }

        public void StartStemming()
        {
            var index = fileProvider.GetTextFromFile($"{ParentForderPath}/{uri.Host}/index.txt");
            //пути к фалам
            var files = index.Split("\n").Select(x => x.Split(" ")).Select(x => x[2]).ToList();
            //приводим к начальной форме
            foreach (var file in files)
            {
                StemmingFile(file);
            }
        }

        //токенизация
        private void StemmingFile(string path)
        {
            var text = fileProvider.GetTextFromFile(path);
            var words = new List<string>();
            MatchCollection collection = Regex.Matches(text, @"([\w]{1,})");
            var porter = new Porter();
            foreach (Match word in collection)
            {
                string stremmed;
                if (word.Value.Length > 3)
                {
                    //лемматизация
                    stremmed = porter.Stemm(word.Value);
                }
                else
                {
                    stremmed = word.Value;
                }
                words.Add(stremmed);
            }
            var filename = Regex.Match(path, @"([\d]*.txt)");
            fileProvider.WriteTextToFile($"{ParentForderPath}/{uri.Host}/stemming/{filename.Value}", string.Join(' ', words));
        }
    }
}
