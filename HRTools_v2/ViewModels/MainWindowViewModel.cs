using Domain.Data;
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
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HRTools_v2.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Properties

        #region UI States

        private UIComponentState _searchComponentState;
        public UIComponentState SearchComponentState
        {
            get => _searchComponentState;
            set { SetProperty(ref _searchComponentState, value); }
        }

        private UIComponentState _menuComponentState;
        public UIComponentState MenuComponentState
        {
            get => _menuComponentState;
            set { SetProperty(ref _menuComponentState, value); }
        }

        private UIComponentState _rosterComponentState;
        public UIComponentState RosterComponentState
        {
            get => _rosterComponentState;
            set { SetProperty(ref _rosterComponentState, value); }
        }

        private UIComponentState _employeeComponentState;
        public UIComponentState EmployeeComponentState
        {
            get => _employeeComponentState;
            set { SetProperty(ref _employeeComponentState, value); }
        }

        private bool _isMainLoaderVisible;
        public bool IsMainLoaderVisible
        {
            get => _isMainLoaderVisible;
            set { SetProperty(ref _isMainLoaderVisible, value); }
        }

        #endregion

        #region Search

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); HandleSearch(); }
        }

        private ObservableCollection<Roster> _suggestionList;
        public ObservableCollection<Roster> SuggestionList
        {
            get { return _suggestionList; }
            set { SetProperty(ref _suggestionList, value); }
        }

        #endregion

        #endregion

        #region Delegates

        private DelegateCommand<string> _navigateCommand = null;
        public DelegateCommand<string> NavigateCommand => _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(Navigate));

        private DelegateCommand<Roster> _onSearchItemClickedCommand = null;
        public DelegateCommand<Roster> OnSearchItemClickedCommand => _onSearchItemClickedCommand ?? (_onSearchItemClickedCommand = new DelegateCommand<Roster>(OnSearchItemClicked));

        private DelegateCommand _exportRosterToCSVCommand = null;
        public DelegateCommand ExportRosterToCSVCommand => _exportRosterToCSVCommand ?? (_exportRosterToCSVCommand = new DelegateCommand(ExportRosterToCSV));

        private DelegateCommand _loadRosterFromCSVCommand = null;
        public DelegateCommand LoadRosterFromCSVCommand => _loadRosterFromCSVCommand ?? (_loadRosterFromCSVCommand = new DelegateCommand(LoadRosterFromCSV));

        private DelegateCommand _getRosterFromWebCommand = null;
        public DelegateCommand GetRosterFromWebCommand => _getRosterFromWebCommand ?? (_getRosterFromWebCommand = new DelegateCommand(GetRosterFromWeb));

        #endregion

        private readonly SearchProvider<Roster> _dataProvider;
        private RosterDataManager _rosterDataManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private string _currentRegion;

        public MainWindowViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            SearchComponentState = UIComponentState.Hidden;
            RosterComponentState = UIComponentState.Visible;
            EmployeeComponentState = UIComponentState.Hidden;
            MenuComponentState = UIComponentState.Hidden;

            IsMainLoaderVisible = false;

            SuggestionList = new ObservableCollection<Roster>();
            _dataProvider = new SearchProvider<Roster>();
            _rosterDataManager = new RosterDataManager();

            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _currentRegion = "LoaderPage";

            _ = eventAggregator.GetEvent<NavigationArgs>().Subscribe(Navigate);
            _ = eventAggregator.GetEvent<NavigationEmplArgs>().Subscribe(Navigate);
            _ = eventAggregator.GetEvent<MainLoaderVisibilityArgs>().Subscribe(SetLoaderVisibility);
        }

        #region Roster Methods

        private async void LoadRosterFromCSV()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv");
            var path = dialog.ShowOpenDialog();
            if (string.IsNullOrEmpty(path)) return;

            RosterComponentState = UIComponentState.Loading;
            if (DataStorage.RosterList != null && DataStorage.RosterList.Count > 0) DataStorage.RosterList.Clear();

            var csvStream = new CSVStream(path);
            var rosterList = await _rosterDataManager.GetRosterAsync(csvStream);
            DataStorage.RosterList.AddRange(rosterList);
            RosterComponentState = UIComponentState.Visible;
        }

        private async void ExportRosterToCSV()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save roster data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            RosterComponentState = UIComponentState.Loading;
            var csvStream = new CSVStream(path);
            await _rosterDataManager.WriteRosterAsync(csvStream, DataStorage.RosterList);
            RosterComponentState = UIComponentState.Visible;

            FileHelper.RunProcess(path);
        }

        private async void GetRosterFromWeb()
        {
            RosterComponentState = UIComponentState.Loading;

            if (DataStorage.RosterList != null && DataStorage.RosterList.Count > 0) DataStorage.RosterList.Clear(); 

            var results = await _rosterDataManager.GetWebRosterAsync(new WebStream(), DataStorage.AppSettings.RosterURL);
            DataStorage.RosterList.AddRange(results);

            RosterComponentState = UIComponentState.Visible;
           
        }

        #endregion

        #region Search Methods

        public async void HandleSearch()
        {
            SuggestionList.Clear();

            if (string.IsNullOrEmpty(SearchText)){
                SearchComponentState = UIComponentState.Hidden;
                return;
            }

            SearchComponentState = UIComponentState.Loading;

            if (_dataProvider.IsSearching)
            {
                _dataProvider.SetValues(SearchText);
                return;
            }

            _dataProvider.SetValues(SearchText, DataStorage.RosterList);
            var result = await _dataProvider.LookUpAsync();

            UpdateList(result);
            
        }

        private void UpdateList(List<Roster> list)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                if (list.Count > 50)
                {
                    SuggestionList.AddRange(list.GetRange(0, 49));
                }
                else
                {
                    SuggestionList.AddRange(list);
                }

                SearchComponentState = list.Count > 0 ? UIComponentState.Visible : UIComponentState.Empty;
            }
        }

        private void OnSearchItemClicked(Roster selectedEmployee)
        {
            SearchComponentState = UIComponentState.Hidden;
            SuggestionList.Clear();
            Navigate(("EmployeeData", selectedEmployee));
            //_eventAggregator.GetEvent<NavigationArgs>().Publish(selectedEmployee);
        }

        #endregion

        #region Navigation

        private void Navigate((string, Roster) data)
        {
            if (_currentRegion.Equals(data.Item1) && !_currentRegion.Equals("EmployeeData")) return;

            NavigationParameters param = new NavigationParameters
            {
                { "selectedEmployee", data.Item2 }
            };

            if (data.Item1 != null)
            {
                //if (_currentRegion.Equals("LoaderPage")) MenuComponentState = UIComponentState.Visible;
                _currentRegion = data.Item1;
                _regionManager.RequestNavigate("ContentRegion", data.Item1, param);
            }
        }

        private void Navigate(string navigatePath)
        {
            if (_currentRegion.Equals(navigatePath) && !_currentRegion.Equals("EmployeeData")) return;
            if (navigatePath != null)
            {
                //if (_currentRegion.Equals("LoaderPage")) MenuComponentState = UIComponentState.Visible;
                _currentRegion = navigatePath;
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
            }
        }

        #endregion

        private void SetLoaderVisibility(bool v)
        {
            IsMainLoaderVisible = v;
        }
    }
}
