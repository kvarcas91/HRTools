using Domain.Data;
using Domain.Factory;
using Domain.IO;
using Domain.Models;
using Domain.Networking;
using Domain.Storage;
using Domain.Types;
using HRTools_v2.Args;
using HRTools_v2.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Linq;

namespace HRTools_v2.ViewModels
{
    public class LoaderPageViewModel : BindableBase
    {

        #region UI States

        private LoadingPageState _loadingPageUIState;
        public LoadingPageState LoadingPageUIState
        {
            get => _loadingPageUIState;
            set { SetProperty(ref _loadingPageUIState, value); }
        }

        #endregion

        private string _mainLoaderText;
        public string MainLoaderText
        {
            get => _mainLoaderText;
            set { SetProperty(ref _mainLoaderText, value); }
        }

        private string _siteIdText;
        public string SiteIdText
        {
            get => _siteIdText;
            set { SetProperty(ref _siteIdText, value); }
        }

        #region Delegates

        private DelegateCommand _setSiteIDCommand = null;
        public DelegateCommand SetSiteIDCommand => _setSiteIDCommand ?? (_setSiteIDCommand = new DelegateCommand(SetSiteID));

        private DelegateCommand _selectDbFolderCommand = null;
        public DelegateCommand SelectDbFolderCommand => _selectDbFolderCommand ?? (_selectDbFolderCommand = new DelegateCommand(SelectDbPath));

        private DelegateCommand _skipRosterLoadCommand = null;
        public DelegateCommand SkipRosterLoadCommand => _skipRosterLoadCommand ?? (_skipRosterLoadCommand = new DelegateCommand(DbHealthCheck));

        private DelegateCommand _loadRosterFromCSVCommand = null;
        public DelegateCommand LoadRosterFromCSVCommand => _loadRosterFromCSVCommand ?? (_loadRosterFromCSVCommand = new DelegateCommand(LoadRosterFromCSV));

        #endregion

        private RosterDataManager _rosterDataManager;
        private SettingsManager _settingsManager;
        private readonly IEventAggregator _eventAggregator;

        public LoaderPageViewModel(IEventAggregator eventAggregator)
        {
            _rosterDataManager = new RosterDataManager();
            _eventAggregator = eventAggregator;

            SetLoaderFlow();
        }

        private void SetLoaderFlow()
        {
            SetSettings();
        }

        private void SetSettings()
        {
            LoadingPageUIState = LoadingPageState.SettingsLoading;
            MainLoaderText = "Loading Settings...";
            _settingsManager = new SettingsManager();
            DataStorage.AppSettings = _settingsManager.Init();
            if (string.IsNullOrEmpty(DataStorage.AppSettings.SiteID))
            {
                MainLoaderText = "It seems that you don't have home site set. Please enter below your site ID and try again.";
                LoadingPageUIState = LoadingPageState.SettingsFailedToLoad;
            }
            else
            {
                GetRosterFromWeb();
            }
        }

        private void SetSiteID()
        {
            if (string.IsNullOrEmpty(SiteIdText) || SiteIdText.Length > 4) return;

            _settingsManager.Set("SiteID", SiteIdText.ToUpper());
            DataStorage.AppSettings = _settingsManager.Init();

            GetRosterFromWeb();
        }

        private void ShowManualRosterUploadComponent()
        {
            LoadingPageUIState = LoadingPageState.RosterFailedToLoad;
            MainLoaderText = $"Unfortunately, 'HR Tools' failed to get {DataStorage.AppSettings.SiteID} roster. Do you want to load it manually? If not - click \"Skip\" (Note! if you skip roster upload, access to this application will be limited)";
        }

        private async void GetRosterFromWeb()
        {

            LoadingPageUIState = LoadingPageState.RosterLoading;

            if (string.IsNullOrEmpty(DataStorage.AppSettings.SiteID)) return;
            if (DataStorage.RosterList != null && DataStorage.RosterList.Count > 0) DataStorage.RosterList.Clear();

            MainLoaderText = $"Hold on for a second. Trying to get {DataStorage.AppSettings.SiteID} roster";
            var results = await _rosterDataManager.GetWebRosterAsync(new WebStream(), DataStorage.AppSettings.RosterURL.Replace("{siteID}", DataStorage.AppSettings.SiteID));
            DataStorage.RosterList.AddRange(results);

            if (results == null || results.Count == 0)
            {
                ShowManualRosterUploadComponent();
            }
            else
            {
                DbHealthCheck();
            }
        }

        private async void LoadRosterFromCSV()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv");
            var path = dialog.ShowOpenDialog();
            if (string.IsNullOrEmpty(path)) return;

            
            MainLoaderText = $"Hold on for a second. Parsing {DataStorage.AppSettings.SiteID} roster";
            LoadingPageUIState = LoadingPageState.RosterLoading;

            if (DataStorage.RosterList != null && DataStorage.RosterList.Count > 0) DataStorage.RosterList.Clear();

            var csvStream = new CSVStream(path);
            var dataMap = new DataMap(new RosterImportMap(), DataImportType.Roster);
            var rosterList = await csvStream.GetAsync(dataMap);

            DataStorage.RosterList.AddRange(rosterList.Cast<Roster>().ToList());

            if (rosterList == null || rosterList.Count == 0)
            {
                ShowManualRosterUploadComponent();
            }
            else
            {
                DbHealthCheck();
            }
        }

        private async void DbHealthCheck()
        {
            MainLoaderText = $"Checking database connection...";
            LoadingPageUIState = LoadingPageState.DbLoading;

            var dbPath = Environment.UserName.Equals("eslut") ? DataStorage.AppSettings.DbTestPath : DataStorage.AppSettings.DbProductionPath;

            if (string.IsNullOrEmpty(dbPath)) SetDbPath();
            else
            {
                var isDbExists = await FileHelper.CheckDbHealthAsync(dbPath);
                if (!isDbExists)
                {
                    LoadingPageUIState = LoadingPageState.DbFailedToLoad;
                    MainLoaderText = $"Failed to create database... Please contact sslts@";
                    return;
                }

                CasheDatabase();
            }

        }

        private void SetDbPath()
        {
            LoadingPageUIState = LoadingPageState.DbPathFailedToLoad;
            MainLoaderText = $"We cannot find your database location.. Please point to the right direction! If you don't have Database file created - please create a folder in Support/HR shared folder named \"Database\" and select it";
        }

        private async void SelectDbPath()
        {
            var dlg = new FolderPicker
            {
                InputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if (dlg.ShowDialog() == true)
            {
                string dbPathType = Environment.UserName.Equals("eslut") ? "DbTestPath" : "DbProductionPath";
                _settingsManager.Set(dbPathType, $@"{dlg.ResultPath}\db.db");
                DataStorage.AppSettings = _settingsManager.Init();

                var dbPath = Environment.UserName.Equals("eslut") ? DataStorage.AppSettings.DbTestPath : DataStorage.AppSettings.DbProductionPath;
                var isDbExists = await FileHelper.CheckDbHealthAsync(dbPath);
                if (!isDbExists)
                {
                    LoadingPageUIState = LoadingPageState.DbFailedToLoad;
                    MainLoaderText = $"Failed to create database... Please contact sslts@";
                    return;
                }

                CasheDatabase();
            }
        }

        private async void CasheDatabase()
        {
            MainLoaderText = "Caching database";
            LoadingPageUIState = LoadingPageState.DbLoading;

            CacheManager.MoveFrom = Environment.UserName.Equals("eslut") ? DataStorage.AppSettings.DbTestPath : DataStorage.AppSettings.DbProductionPath;
            CacheManager.MoveTo = DataStorage.AppSettings.LocalDbPath;
            _ = await FileHelper.CreateDirectoryIfNotExistsAsync(DataStorage.AppSettings.LocalDbPath);
            CacheManager.Initialize(DataStorage.AppSettings.CacheRefreshRateInSeconds, visibility => _eventAggregator.GetEvent<MainLoaderVisibilityArgs>().Publish(visibility));

            MainLoaderText = string.Empty;
            DataStorage.CanLoad = true;
            _eventAggregator.GetEvent<NavigationArgs>().Publish("HomePage");
        }

    }
}
