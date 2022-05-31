using Domain.DataManager;
using Domain.Factory;
using Domain.IO;
using Domain.Models;
using Domain.Models.Meetings;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using HRTools_v2.Args;
using HRTools_v2.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace HRTools_v2.ViewModels.Meetings
{
    public class MeetingsPageViewModel : BindableBase, INavigationAware
    {

        private bool _isCurrentPage;

        private HomePageWidgetState _widgedState;
        public HomePageWidgetState WidgedState
        {
            get => _widgedState;
            set { SetProperty(ref _widgedState, value); }
        }

        private bool _hasData;
        public bool HasData
        {
            get => _hasData;
            set { SetProperty(ref _hasData, value); }
        }

        private bool _isMeetingImportLoading;
        public bool IsMeetingImportLoading
        {
            get => _isMeetingImportLoading;
            set { SetProperty(ref _isMeetingImportLoading, value); }
        }

        private string _selectedMeetingStatus;
        public string SelectedMeetingStatus
        {
            get => _selectedMeetingStatus;
            set { SetProperty(ref _selectedMeetingStatus, value); GetData(); }
        }

        private string _selectedMeetingType;
        public string SelectedMeetingType
        {
            get => _selectedMeetingType;
            set { SetProperty(ref _selectedMeetingType, value); GetData(); }
        }

        private ObservableCollection<string> _meetingStatusList;
        public ObservableCollection<string> MeetingStatusList
        {
            get { return _meetingStatusList; }
            set { SetProperty(ref _meetingStatusList, value); }
        }

        private ObservableCollection<string> _selectedMeetingTypeList;
        public ObservableCollection<string> SelectedMeetingTypeList
        {
            get { return _selectedMeetingTypeList; }
            set { SetProperty(ref _selectedMeetingTypeList, value); }
        }

        private ObservableCollection<MeetingsEntity> _meetingsList;
        public ObservableCollection<MeetingsEntity> MeetingsList
        {
            get { return _meetingsList; }
            set { SetProperty(ref _meetingsList, value); }
        }

        private string _selectedManager;
        public string SelectedManager
        {
            get => _selectedManager;
            set { SetProperty(ref _selectedManager, value); GetData(); }
        }

        private ObservableCollection<string> _managerList;
        public ObservableCollection<string> ManagerList
        {
            get { return _managerList; }
            set { SetProperty(ref _managerList, value); }
        }

        private readonly MeetingsRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        #region Delegages

        private DelegateCommand _importMeetingsFileCommand = null;
        public DelegateCommand ImportMeetingsFileCommand => _importMeetingsFileCommand ?? (_importMeetingsFileCommand = new DelegateCommand(ImportMeetings));

        private DelegateCommand<MeetingsEntity> _openEmployeeViewCommand = null;
        public DelegateCommand<MeetingsEntity> OpenEmployeeViewCommand => _openEmployeeViewCommand ?? (_openEmployeeViewCommand = new DelegateCommand<MeetingsEntity>(OpenEmployeeView));

        private DelegateCommand _alignAMCommand = null;
        public DelegateCommand AlignAMCommand => _alignAMCommand ?? (_alignAMCommand = new DelegateCommand(AlignAM));

        #endregion

        public MeetingsPageViewModel(IEventAggregator eventAggregator)
        {
            IsMeetingImportLoading = false;
            _eventAggregator = eventAggregator;
            _repository = new MeetingsRepository();
            MeetingsList = new ObservableCollection<MeetingsEntity>();
            SelectedManager = "All";
            _selectedMeetingStatus = "Open/Pending";
            MeetingStatusList = new ObservableCollection<string> { "Open/Pending", "Closed"};
            SelectedMeetingTypeList = new ObservableCollection<string> { "All", "Outstanding" };
            _selectedMeetingType = "All";
            ManagerList = new ObservableCollection<string>();
            SetManagerNames();
        }


        private async void GetData()
        {
            if (!_isCurrentPage) return;

            MeetingsList.Clear();
            Console.WriteLine("GetData");

            WidgedState = HomePageWidgetState.SummaryLoading;

            var data = await _repository.GetMeetingsAsync(SelectedMeetingStatus, SelectedManager, SelectedMeetingType);
            foreach (var item in data)
            {
                item.SetProgress();
            }
            MeetingsList.AddRange(data);
            HasData = MeetingsList.Count > 0;

            WidgedState = HomePageWidgetState.SummaryLoaded;
        }

        private void OpenEmployeeView(MeetingsEntity meeting)
        {
            if (meeting == null) return;
            var empl = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(meeting.EmployeeID)).FirstOrDefault();
            if (empl == null)
            {
                empl = new Roster(meeting);
            }

            _eventAggregator.GetEvent<NavigationEmplArgs>().Publish(("EmployeeData", empl));
        }

        private async void ImportMeetings()
        {

            var dialog = new DialogHelper(".csv", "CSV Files|*.csv");
            var path = dialog.ShowOpenDialog();
            if (string.IsNullOrEmpty(path)) return;

            IsMeetingImportLoading = true;

            var allERMeetings = await _repository.GetMeetingsAsync();
            var dataMap = new DataMap(new ERMeetingImportMap(), DataImportType.ERMeeting);
            var csvReader = new CSVStream(path);
            var csvOutput = csvReader.Get(dataMap);
            var importManager = new ERMeetingsImportManager();
            var dataList = await importManager.GetERMeetingsAsync(csvOutput, allERMeetings);

            var updateList = dataList.Item1;
            var insertList = dataList.Item2;

            if (insertList.Count > 0)
            {
                var results = await _repository.InsertAllAsync(insertList);
                if (!results.Success) SendToast("Failed to upload new Meetings to database", NotificationType.Error);
                else SendToast($"{insertList.Count} Meetings loaded successfully!", NotificationType.Success);
            }

            if (updateList.Count > 0)
            {
                var results = await _repository.UpdateAllAsync(updateList);
                if (!results.Success) SendToast("Failed to update existing Meetings to database", NotificationType.Error);
                else SendToast($"{updateList.Count} Existing Meetings updated successfully!", NotificationType.Success);
            }

            if (insertList.Count == 0 && updateList.Count == 0) SendToast($"Meetings are up to date", NotificationType.Information);
            else GetData();

            IsMeetingImportLoading = false;
        }

        private async void AlignAM()
        {
            if (DataStorage.RosterList == null || DataStorage.RosterList.Count == 0) return;
            WidgedState = HomePageWidgetState.SummaryLoading;

            var dataManager = new ERMeetingsImportManager();
            var openMeetings = await _repository.GetMeetingsAsync("Open/Pending", "All", "All");
            var updateList = await dataManager.AlignAM(openMeetings);
            if (updateList.Count == 0)
            {
                WidgedState = HomePageWidgetState.SummaryLoaded;
                SendToast("Nothing to update", NotificationType.Information);
                return;
            }

            var results = await _repository.UpdateAllAsync(updateList);
            if (!results.Success) SendToast("Failed to allign managers", NotificationType.Error);
            else SendToast($"{updateList.Count} meeting(s) have been updated!", NotificationType.Success);

            WidgedState = HomePageWidgetState.SummaryLoaded;

        }

        private async void SetManagerNames()
        {
            ManagerList.Clear();
            ManagerList.Add("All");
            ManagerList.AddRange(await _repository.GetMeetingsDistinctManagersAsync());
        }

        private void SendToast(string message, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<ShowToastArgs>().Publish((message, notificationType));
        }

        #region Navigation

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _isCurrentPage = false;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _isCurrentPage = true;
            GetData();

        }

        #endregion
    }
}
