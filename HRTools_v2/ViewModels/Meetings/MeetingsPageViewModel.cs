using Domain.Data;
using Domain.DataManager;
using Domain.Extensions;
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HRTools_v2.ViewModels.Meetings
{
    public class MeetingsPageViewModel : BindableBase, INavigationAware
    {

        #region Search

        private UIComponentState _searchComponentState;
        public UIComponentState SearchComponentState
        {
            get => _searchComponentState;
            set { SetProperty(ref _searchComponentState, value); }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); HandleSearch(); }
        }

        private ObservableCollection<MeetingsEntity> _suggestionList;
        public ObservableCollection<MeetingsEntity> SuggestionList
        {
            get { return _suggestionList; }
            set { SetProperty(ref _suggestionList, value); }
        }

        #endregion

        private string _reasonForErClosure;
        public string ReasonForErClosure
        {
            get => _reasonForErClosure;
            set { SetProperty(ref _reasonForErClosure, value); }
        }

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
        private readonly SearchProvider<MeetingsEntity> _dataProvider;

        #region Delegages

        private DelegateCommand _importMeetingsFileCommand = null;
        public DelegateCommand ImportMeetingsFileCommand => _importMeetingsFileCommand ?? (_importMeetingsFileCommand = new DelegateCommand(ImportMeetings));

        private DelegateCommand _exportOutstandingMeetingsCommand = null;
        public DelegateCommand ExportOutstandingMeetingsCommand => _exportOutstandingMeetingsCommand ?? (_exportOutstandingMeetingsCommand = new DelegateCommand(ExportOutstandingMeetings));

        private DelegateCommand _exportMeetingsCommand = null;
        public DelegateCommand ExportMeetingsCommand => _exportMeetingsCommand ?? (_exportMeetingsCommand = new DelegateCommand(ExportMeetings));

        private DelegateCommand _exportSelectedMeetingsCommand = null;
        public DelegateCommand ExportSelectedMeetingsCommand => _exportSelectedMeetingsCommand ?? (_exportSelectedMeetingsCommand = new DelegateCommand(ExportSelectedMeetings));

        private DelegateCommand<MeetingsEntity> _openEmployeeViewCommand = null;
        public DelegateCommand<MeetingsEntity> OpenEmployeeViewCommand => _openEmployeeViewCommand ?? (_openEmployeeViewCommand = new DelegateCommand<MeetingsEntity>(OpenEmployeeView));

        private DelegateCommand _alignAMCommand = null;
        public DelegateCommand AlignAMCommand => _alignAMCommand ?? (_alignAMCommand = new DelegateCommand(AlignAM));

        private DelegateCommand<MeetingsEntity> _onErMeetingEditCommand = null;
        public DelegateCommand<MeetingsEntity> OnErMeetingEditCommand => _onErMeetingEditCommand ?? (_onErMeetingEditCommand = new DelegateCommand<MeetingsEntity>(EditErMeeting));

        private DelegateCommand<MeetingsEntity> _onMeetingCancelCommand = null;
        public DelegateCommand<MeetingsEntity> OnMeetingCancelCommand => _onMeetingCancelCommand ?? (_onMeetingCancelCommand = new DelegateCommand<MeetingsEntity>(CancelErMeeting));

        private DelegateCommand<MeetingsEntity> _setMeetingPendingCommand = null;
        public DelegateCommand<MeetingsEntity> SetMeetingPendingCommand => _setMeetingPendingCommand ?? (_setMeetingPendingCommand = new DelegateCommand<MeetingsEntity>(SetMeetingPending));

        private DelegateCommand<MeetingsEntity> _reopenMeetingCommand = null;
        public DelegateCommand<MeetingsEntity> ReopenMeetingCommand => _reopenMeetingCommand ?? (_reopenMeetingCommand = new DelegateCommand<MeetingsEntity>(ReopenMeeting));

        #endregion

        public MeetingsPageViewModel(IEventAggregator eventAggregator)
        {
            SearchComponentState = UIComponentState.Hidden;
            _dataProvider = new SearchProvider<MeetingsEntity>();
            SuggestionList = new ObservableCollection<MeetingsEntity>();
            IsMeetingImportLoading = false;
            _eventAggregator = eventAggregator;
            _repository = new MeetingsRepository();
            MeetingsList = new ObservableCollection<MeetingsEntity>();
            SelectedManager = "All";
            _selectedMeetingStatus = "Open/Pending";
            MeetingStatusList = new ObservableCollection<string> { "Open/Pending", "Open", "Pending", "Closed"};
            SelectedMeetingTypeList = new ObservableCollection<string> { "All", "Outstanding" };
            _selectedMeetingType = "All";
            ManagerList = new ObservableCollection<string>();
            SetManagerNames();
        }


        private async void GetData()
        {
            if (!_isCurrentPage) return;

            MeetingsList.Clear();

            WidgedState = HomePageWidgetState.SummaryLoading;

            var data = await _repository.GetMeetingsAsync(SelectedMeetingStatus, SelectedManager, SelectedMeetingType);
            foreach (var item in data)
            {
                item.SetProgress();
                item.SetAge();
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

        private async void EditErMeeting(MeetingsEntity meeting)
        {
            var previewRepo = new PreviewRepository();
            var EmployeeLiveSanctions = await previewRepo.GetLiveSanctionsPreviewAsync(meeting.EmployeeID);
            var sanction = meeting.MeetingType == MeetingType.Health ? EmployeeLiveSanctions.HealthSanction : EmployeeLiveSanctions.DisciplinarySanction;
            if (SanctionManager.IsLesser(sanction, meeting.SecondMeetingOutcome))
            {
                SendToast("You cannot issue lesser sanction!", NotificationType.Information);
                return;
            }

            var repo = new MeetingsRepository();
            var response = await repo.UpdateAsync(meeting);
            if (response.Success)
            {
                SendToast("Meeting has been updated!", NotificationType.Success);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
        }

        private async void SetMeetingPending(MeetingsEntity meeting)
        {
            var response = await _repository.ChangeMeetingStatusAsync(meeting, "Pending");

            if (response.Success)
            {
                SendToast("Meeting status has been updated!", NotificationType.Success);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
        }

        private async void ReopenMeeting(MeetingsEntity meeting)
        {
            var response = await _repository.ChangeMeetingStatusAsync(meeting, "Open");

            if (response.Success)
            {
                SendToast("Meeting status has been updated!", NotificationType.Success);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
        }

        private async void CancelErMeeting(MeetingsEntity meeting)
        {
            if (string.IsNullOrEmpty(ReasonForErClosure))
            {
                SendToast("Reason is mandatory!", NotificationType.Information);
                return;
            }

            if (meeting.MeetingStatus != "Open" && meeting.MeetingStatus != "Pending")
            {
                SendToast("Meeting is already closed!", NotificationType.Information);
                return;
            }

            var meetingRepo = new MeetingsRepository();
            var response = await meetingRepo.CloseERMeeting(meeting, ReasonForErClosure);
            if (response.Success)
            {
                SendToast("Meeting has been closed!", NotificationType.Success);
                ReasonForErClosure = string.Empty;
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }

        }

        private void SendToast(string message, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<ShowToastArgs>().Publish((message, notificationType));
        }

        #region Exports

        private async void ExportOutstandingMeetings()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save meetings data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            var list = await _repository.GetOutstandingMeetingsAsync();
            Export(path, list.OrderBy(x => x.ManagerName));
        }

        private async void ExportMeetings()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save meetings data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            var list = await _repository.GetMeetingsAsync();
            Export(path, list);
        }

        private void ExportSelectedMeetings()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save meetings data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            Export(path, MeetingsList);
        }

        private async void Export(string path, IEnumerable<MeetingsEntity> list)
        {
            var csvStream = new CSVStream(path);
            var dataManager = new DataManager();
            foreach (var item in list)
            {
                item.SetProgress();
                item.SetAge();
            }
            await dataManager.WriteToCsvAsync(csvStream, list);

            FileHelper.RunProcess(path);
        }

        #endregion

        #region Search

        public async void HandleSearch()
        {
            SuggestionList.Clear();

            if (string.IsNullOrEmpty(SearchText))
            {
                SearchComponentState = UIComponentState.Hidden;
                return;
            }

            SearchComponentState = UIComponentState.Loading;

            if (_dataProvider.IsSearching)
            {
                _dataProvider.SetValues(SearchText);
                return;
            }

            _dataProvider.SetValues(SearchText);
            var result = await _dataProvider.LookUpAsync();

            UpdateList(result);

        }

        private void UpdateList(List<MeetingsEntity> list)
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

        

        #endregion

        #region Navigation

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _isCurrentPage = false;
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _isCurrentPage = true;
            var data = await _repository.GetMeetingsAsync();
            foreach (var item in data)
            {
                item.SetProgress();
                item.SetAge();
            }
            _dataProvider.SetList(data);
            GetData();

        }

        #endregion
    }
}
