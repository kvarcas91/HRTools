using Domain.Factory;
using Domain.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Domain.IO
{
    public class CSVStream : IDataStream 
    {

        private string _path;

        public CSVStream(string path)
        {
            _path = path;
        }

        public IList<IDataImportObject> Get(DataMap dataMap) 
        {
            var outputList = new List<IDataImportObject>();
            string[] headers;

            using (TextFieldParser csvParser = new TextFieldParser(_path))
            {
                csvParser.CommentTokens = new string[] { "#" };
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.HasFieldsEnclosedInQuotes = true;

                headers = csvParser.ReadLine().Replace("\"", "").Replace(" ", "").Split(',');
                dataMap.SetDataMap(headers);
                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();
                    var dataFactory = new DataImportObjectFactory().Create(fields, dataMap);
                    outputList.Add(dataFactory);
                }
            }
            return outputList;
        }

        public Task<IList<IDataImportObject>> GetAsync(DataMap dataMap)
        {
            return Task.Run(() => Get(dataMap));
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
                        stream.WriteLine(item.GetDataHeader());
                        isHeaderWritten = true;
                    }

                    stream.WriteLine(item.GetDataRow());
                }
            }

        }
    }
}
