﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Crawler
{
    public class FileProvider
    {
        public string GetTextFromFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteTextToFile(string path, string text)
        {
            using (FileStream fs = File.Create(path))
            {
                Byte[] title = new UTF8Encoding(true).GetBytes(text);
                fs.Write(title, 0, title.Length);
            }
        }
    }
}
