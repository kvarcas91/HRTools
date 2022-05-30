using Domain.Models;
using Domain.Models.Meetings;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using HRTools_v2.Args;
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

        private ObservableCollection<string> _meetingStatusList;
        public ObservableCollection<string> MeetingStatusList
        {
            get { return _meetingStatusList; }
            set { SetProperty(ref _meetingStatusList, value); }
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

        #endregion

        public MeetingsPageViewModel(IEventAggregator eventAggregator)
        {
            IsMeetingImportLoading = false;
            _eventAggregator = eventAggregator;
            _repository = new MeetingsRepository();
            MeetingsList = new ObservableCollection<MeetingsEntity>();
            _selectedManager = "All";
            _selectedMeetingStatus = "Open/Pending";
            MeetingStatusList = new ObservableCollection<string> { "Open/Pending", "Closed"};
            ManagerList = new ObservableCollection<string>();
        }


        private async void GetData()
        {
            if (!_isCurrentPage) return;

            MeetingsList.Clear();

            WidgedState = HomePageWidgetState.SummaryLoading;

            var data = await _repository.GetMeetingsAsync(SelectedMeetingStatus, SelectedManager);
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
            IsMeetingImportLoading = true;
        }

        private async void SetManagerNames()
        {
            ManagerList.Clear();
            ManagerList.Add("All");
            ManagerList.AddRange(await _repository.GetMeetingsDistinctManagersAsync());
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
            SetManagerNames();
            SelectedManager = "All";
            //GetData();
        }

        #endregion
    }
}
