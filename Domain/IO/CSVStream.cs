using Domain.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Domain.IO
{
    public class CSVStream : IDataStream 
    {

        private string _path;

        public CSVStream(string path)
        {
            _path = path;
        }

        public IList<T> Get<T>(string[] requiredHeaders, Func<string[], Dictionary<string,int>, T> createNewObj) where T : class, new()
        {
            List<T> outputList = new List<T>();
            string[] headers;

            using (TextFieldParser csvParser = new TextFieldParser(_path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                headers = csvParser.ReadLine().Replace("\"", "").Replace(" ", "").Split(',');
                var map = GetMap(headers, requiredHeaders);
                if (map.Count == 0) return outputList;

                while (!csvParser.EndOfData)
                {

                    string[] fields = csvParser.ReadFields();
                    var results = createNewObj(fields, map);
                    if (results != null) outputList.Add(results);
                }
            }
            return outputList;
        }

        public void Write<T>(IEnumerable<T> dataList) where T : IWritable
        {
            using (var stream = File.CreateText(_path))
            {
                bool isHeaderWritten = false;

                foreach (var item in dataList)
                {
                    if (!isHeaderWritten)
                    {
                        stream.WriteLine(item.GetHeader());
                        isHeaderWritten = true;
                    }

                    stream.WriteLine(item.GetRow());
                }
            }

        }

        public Dictionary<string, int> GetMap(string[] headers, string[] requiredHeaders)
        {
            var map = new Dictionary<string, int>();

            foreach (string item in requiredHeaders)
            {
                map.Add(item, Array.IndexOf(headers, item));
            }

            return map;
        }
    }
}
