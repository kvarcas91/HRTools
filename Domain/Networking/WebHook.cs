using Domain.Storage;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Networking
{
    public static class WebHook
    {
        private static readonly HttpClient client = new HttpClient();
        
       
        public static Task<HttpResponseMessage> PostAsync(string url, string data)
        {
            var enabledWebHooks = DataStorage.AppSettings.EnableWebHooks;
            if (!enabledWebHooks) return Task.Run(() => { return new HttpResponseMessage(); });
            var mJson = new StringBuilder("{\"Content\":");
            mJson.Append($"\"{data}\"");
            mJson.Append("}");

            return client.PostAsync(url, new StringContent(mJson.ToString(), Encoding.UTF8, "application/json"));
        }
    }
}
