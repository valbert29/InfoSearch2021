using System;
using System.Linq;
using Crawler.Task_2;
using Crawler.Task_5;
using Crawler.Task4;
using iTextSharp.text.html.simpleparser;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            string link = "https://ru.wikipedia.org/";
            string folderPath = @"D:\IS\";

            //1 задание
            //var crawler = new Crawler.Crawler(folderPath);
            //crawler.Crawl(link);

            //2 задание
            //var stemming = new Stemming(link, folderPath);
            //stemming.StartStemming();

            ////3 задание
            //var inverter = new InvertList(link, folderPath);
            ////inverter.Invert();
            //var search = inverter.Search("оформляет & случае & качестве");
            //Console.WriteLine($"Найдено в документах с индексами: {string.Join(", ", search)}");

            ////4 задание
            var tdidf = new TfIdf(link, folderPath);
            //tdidf.TF();
            //tdidf.Idf();
            //tdidf.TfIdfCalc();

            //5 задание
            var searchResult = new Search(link, folderPath);
            searchResult.SearchWord("патрулирован свободный википедия");
        }
    }
}
