using Domain.Models;
using System;
using System.Configuration;

namespace Domain.Data
{
    public class SettingsManager
    {
        public Settings Init()
        {
            Set("LocalDbPath", $@"{AppContext.BaseDirectory}Database\db.db");
            Set("MeetingContentTestPath", $@"{AppContext.BaseDirectory}.Data");

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = ((AppSettingsSection)configFile.GetSection("Settings")).Settings;

            var output = new Settings();
            try
            {
                output.SiteID = settings["SiteID"].Value;
                output.DbProductionPath = settings["DbProductionPath"].Value;
                output.DbTestPath = settings["DbTestPath"].Value;
                output.LocalDbPath = settings["LocalDbPath"].Value;
                output.CacheRefreshRateInSeconds = Convert.ToInt32(settings["CacheRefreshRateInSeconds"].Value);
                output.UserImgURL = settings["UserImgURL"].Value;
                output.RosterURL = settings["RosterURL"].Value;
                output.PunchURL = settings["PunchURL"].Value;
                output.RosterWebDateFormat = settings["RosterWebDateFormat"].Value;
                output.ResignationsQuicklinkURL = settings["ResignationsQuicklinkURL"].Value;
                output.SecurityChanelWebHook = settings["SecurityChanelWebHook"].Value;
                output.AwalChanelWebHook = settings["AwalChanelWebHook"].Value;
                output.TestWebHook = settings["TestWebHook"].Value;
                output.MeetingContentProductionPath = settings["MeetingContentProductionPath"].Value;
                output.MeetingContentTestPath = settings["MeetingContentTestPath"].Value;
            }
            catch
            {
                return output;
            }

            return output;
        }

        public void Set(string key, dynamic value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = ((AppSettingsSection)configFile.GetSection("Settings")).Settings;

            try
            {
                settings[key].Value = value.ToString();
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch
            {
                return;
            }
        }

    }
}
