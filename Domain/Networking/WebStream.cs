using Domain.DataManager;
using Domain.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Domain.Networking
{
    public class WebStream : IWebStream
    {
        public IList<T> Get<T>(string url, string[] requiredHeaders, Func<string[], Dictionary<string, int>, T> createNewObj) where T : class, new()
        {
            List<T> outputList = new List<T>();
            string[] headers;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = new CookieContainer();
            req.UseDefaultCredentials = true;
            req.PreAuthenticate = true;
            req.MaximumAutomaticRedirections = 100;
            req.ContinueTimeout = 5 * 3 * 1000;
            req.Timeout = 5 * 3 * 1000;
            req.Headers.Add(HttpRequestHeader.Cookie, "security=true");
            req.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse resp = null;

            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch(Exception e)
            {
                LoggerManager.Log("WebStream.Get", e.Message);
                return outputList;
            }
            
            if (!resp.ContentType.Equals("text/csv")) return outputList;

            using (var reader = new StreamReader(resp.GetResponseStream()))
            using (TextFieldParser csvParser = new TextFieldParser(reader))
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
                    if (fields.Length < 2) continue;
                    var results = createNewObj(fields, map);
                    if (results != null) outputList.Add(results);
                }
            }
            return outputList;
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
