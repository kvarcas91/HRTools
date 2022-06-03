using Domain.Data;
using Domain.IO;
using Domain.Models;
using Domain.Models.CustomMeetings;
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
using System.Text;
using System.Threading.Tasks;

namespace HRTools_v2.ViewModels.Meetings
{
    public class CustomMeetingsPageViewModel : BindableBase, INavigationAware
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

        private string _selectedMeetingStatus;
        public string SelectedMeetingStatus
        {
            get => _selectedMeetingStatus;
            set { SetProperty(ref _selectedMeetingStatus, value); GetData(); }
        }

        private ObservableCollection<string> _meetingStatusList;
        public ObservableCollection<string> MeetingStatusList
        {
            get { return _meetingStatusList; }
            set { SetProperty(ref _meetingStatusList, value); }
        }

        private string _selectedMeetingType;
        public string SelectedMeetingType
        {
            get => _selectedMeetingType;
            set { SetProperty(ref _selectedMeetingType, value); GetData(); }
        }

        private ObservableCollection<string> _meetingTypeList;
        public ObservableCollection<string> MeetingTypeList
        {
            get { return _meetingTypeList; }
            set { SetProperty(ref _meetingTypeList, value); }
        }

        private string _selectedHRSupport;
        public string SelectedHRSupport
        {
            get => _selectedHRSupport;
            set { SetProperty(ref _selectedHRSupport, value); GetData(); }
        }

        private ObservableCollection<string> _hrSupportList;
        public ObservableCollection<string> HRSupportList
        {
            get { return _hrSupportList; }
            set { SetProperty(ref _hrSupportList, value); }
        }

        private ObservableCollection<CustomMeetingEntity> _meetingsList;
        public ObservableCollection<CustomMeetingEntity> MeetingsList
        {
            get { return _meetingsList; }
            set { SetProperty(ref _meetingsList, value); }
        }

        #region Delegates

        private DelegateCommand<CustomMeetingEntity> _openEmployeeViewCommand = null;
        public DelegateCommand<CustomMeetingEntity> OpenEmployeeViewCommand => _openEmployeeViewCommand ?? (_openEmployeeViewCommand = new DelegateCommand<CustomMeetingEntity>(OpenEmployeeView));

        private DelegateCommand _exportMeetingsCommand = null;
        public DelegateCommand ExportMeetingsCommand => _exportMeetingsCommand ?? (_exportMeetingsCommand = new DelegateCommand(ExportMeetings));

        private DelegateCommand _exportSelectedMeetingsCommand = null;
        public DelegateCommand ExportSelectedMeetingsCommand => _exportSelectedMeetingsCommand ?? (_exportSelectedMeetingsCommand = new DelegateCommand(ExportSelectedMeetings));

        #endregion

        private readonly MeetingsRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        public CustomMeetingsPageViewModel(IEventAggregator eventAggregator)
        {
            _repository = new MeetingsRepository();
            _eventAggregator = eventAggregator;
            MeetingsList = new ObservableCollection<CustomMeetingEntity>();
            _selectedMeetingStatus = "Open/Pending";
            _selectedHRSupport = "All";
            MeetingStatusList = new ObservableCollection<string> { "Open/Pending", "Open", "Pending", "Closed" };
            HRSupportList = new ObservableCollection<string>();
            _selectedMeetingType = "All";
            MeetingTypeList = new ObservableCollection<string> { "All", "Adapt", "Time Fraud", "Investigation", "Appeal", "Grievance", "Eligibility", "Formal Probation Review", "TWA" };
            SetHRSupportNames();
        }

        private async void GetData()
        {
            if (!_isCurrentPage) return;

            MeetingsList.Clear();

            WidgedState = HomePageWidgetState.SummaryLoading;

            var data = await _repository.GetCustomMeetingsAsync(SelectedMeetingStatus, SelectedHRSupport, SelectedMeetingType);
            foreach (var item in data)
            {
                item.Prepare();
            }
            MeetingsList.AddRange(data);
            HasData = MeetingsList.Count > 0;

            WidgedState = HomePageWidgetState.SummaryLoaded;
        }

        private void OpenEmployeeView(CustomMeetingEntity meeting)
        {
            if (meeting == null) return;
            var empl = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(meeting.ClaimantID)).FirstOrDefault();
            if (empl == null)
            {
                empl = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(meeting.RespondentID)).FirstOrDefault();
            }

            if (empl == null)
            {
                empl = new Roster(meeting);
            }

            _eventAggregator.GetEvent<NavigationEmplArgs>().Publish(("EmployeeData", empl));
        }

        private async void SetHRSupportNames()
        {
            HRSupportList.Clear();
            HRSupportList.Add("All");
            HRSupportList.AddRange(await _repository.GetCustomMeetingsDistinctHRSupportAsync());
        }

        #region Data Export

        private async void ExportMeetings()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save sanctions data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            var list = await _repository.GetCustomMeetingsAsync();
            Export(path, list);
        }

        private void ExportSelectedMeetings()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save sanctions data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            Export(path, MeetingsList);
        }

        private async void Export(string path, IEnumerable<CustomMeetingEntity> list)
        {
            var csvStream = new CSVStream(path);
            var dataManager = new DataManager();
            foreach (var item in list)
            {
                item.Prepare();
            }
            await dataManager.WriteToCsvAsync(csvStream, list);

            FileHelper.RunProcess(path);
        }

        #endregion

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
