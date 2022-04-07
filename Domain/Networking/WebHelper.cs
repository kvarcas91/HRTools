using Domain.DataManager;
using System;
using System.Diagnostics;

namespace Domain.Networking
{
    public static class WebHelper
    {
        public static void OpenLink(string url)
        {
            try
            {
                Process.Start(url.Trim());
            }
            catch (Exception ex)
            {
                LoggerManager.Log("OpenLink", ex.Message);
            }
        }

        public static bool IsLink(string url)
        {
            return url.ToUpper().Contains("AMAZON.COM");
        }
    }
}
