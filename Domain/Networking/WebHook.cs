using Domain.Storage;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Networking
{
    public static class WebHook
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly bool _isEnabled = true;
       
        public static Task<HttpResponseMessage> PostAsync(string url, string data)
        {
            if (!_isEnabled) return Task.Run(() => { return new HttpResponseMessage(); });
            var mJson = new StringBuilder("{\"Content\":");
            mJson.Append($"\"{data}\"");
            mJson.Append("}");

            return client.PostAsync(Environment.UserName == "eslut" ? DataStorage.AppSettings.TestWebHook : url, new StringContent(mJson.ToString(), Encoding.UTF8, "application/json"));
        }
    }
}
