using CsvHelper;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace Domain.Networking
{
    public class WebStream : IWebStream
    {

        public IList<T> Get<T>(string url, Action<CsvReader> map)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = new CookieContainer();
            req.UseDefaultCredentials = true;
            req.PreAuthenticate = true;
            req.MaximumAutomaticRedirections = 100;
            req.ContinueTimeout = 5 * 60 * 1000;
            req.Timeout = 5 * 60 * 1000;
            req.Headers.Add(HttpRequestHeader.Cookie, "security=true");
            req.Credentials = CredentialCache.DefaultCredentials;

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            using (var reader = new StreamReader(resp.GetResponseStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                if (map != null) map.Invoke(csv);
                
                var records = csv.GetRecords<T>();
                if (records == null) return new List<T>();
                try
                {
                    return records.ToList();
                }
                catch
                {
                    return new List<T>();
                }
            }
        }
    }
}
