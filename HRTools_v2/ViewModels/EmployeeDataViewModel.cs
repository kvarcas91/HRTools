using Domain.DataManager;
using Domain.Extensions;
using Domain.Models;
using Domain.Models.AWAL;
using Domain.Models.DataSnips;
using Domain.Models.Sanctions;
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

namespace HRTools_v2.ViewModels
{
    public class EmployeeDataViewModel : BindableBase, INavigationAware
    {

        #region Properties

        private uint _selectedTabIndex;
        public uint SelectedTabIndex
        {
            get => _selectedTabIndex;
            set { SetProperty(ref _selectedTabIndex, value); OnTabChange(value); }
        }

        private bool _isFileSectionVisible;
        public bool IsFileSectionVisible
        {
            get => _isFileSectionVisible;
            set { SetProperty(ref _isFileSectionVisible, value); }
        }

        private Roster _selectedEmployee;
        public Roster SelectedEmployee
        {
            get => _selectedEmployee;
            set { SetProperty(ref _selectedEmployee, value); }
        }

        private EmployeeDataPreview _previewEmplDataSnip;
        public EmployeeDataPreview PreviewEmplDataSnip
        {
            get => _previewEmplDataSnip;
            set { SetProperty(ref _previewEmplDataSnip, value); }
        }

        private SanctionPreview _employeeLiveSanctions;
        public SanctionPreview EmployeeLiveSanctions
        {
            get => _employeeLiveSanctions;
            set { SetProperty(ref _employeeLiveSanctions, value); }
        }

        private ObservableCollection<SanctionEntity> _sanctionsList;
        public ObservableCollection<SanctionEntity> SanctionsList
        {
            get { return _sanctionsList; }
            set { SetProperty(ref _sanctionsList, value); }
        }

        private ObservableCollection<AwalEntity> _awalList;
        public ObservableCollection<AwalEntity> AwalList
        {
            get { return _awalList; }
            set { SetProperty(ref _awalList, value); }
        }

        private ObservableCollection<EmployeeSummary> _timeline;
        public ObservableCollection<EmployeeSummary> Timeline
        {
            get { return _timeline; }
            set { SetProperty(ref _timeline, value); }
        }

        private AwalEntry _awalNewEntry;
        public AwalEntry AwalNewEntry
        {
            get => _awalNewEntry;
            set { SetProperty(ref _awalNewEntry, value); }
        }

        private string _avatar;
        public string Avatar
        {
            get => _avatar;
            set { SetProperty(ref _avatar, value); }
        }

        private EmploymentStatus _emplStatus;
        public EmploymentStatus EmplStatus
        {
            get => _emplStatus;
            set { SetProperty(ref _emplStatus, value); }
        }

        private HomePageWidgetState _widgedState;
        public HomePageWidgetState WidgedState
        {
            get => _widgedState;
            set { SetProperty(ref _widgedState, value); }
        }

        private SanctionWidgetState _sanctionState;
        public SanctionWidgetState SanctionState
        {
            get => _sanctionState;
            set { SetProperty(ref _sanctionState, value); }
        }

        #region Sanction Props

        private List<string> _sanctionList;
        public List<string> SanctionList
        {
            get => _sanctionList;
            set { SetProperty(ref _sanctionList, value); }
        }

        private string _selectedSanction;
        public string SelectedSanction
        {
            get => _selectedSanction;
            set { SetProperty(ref _selectedSanction, value); }
        }

        private DateTime _sanctionStartDate;
        public DateTime SanctionStartDate
        {
            get => _sanctionStartDate;
            set { SetProperty(ref _sanctionStartDate, value); }
        }

        #endregion

        #region Data List Count Properties

        private bool _hasSanctionData = true;
        public bool HasSanctionData
        {
            get => _hasSanctionData;
            set { SetProperty(ref _hasSanctionData, value); }
        }

        private bool _hasAwalData = true;
        public bool HasAwalData
        {
            get => _hasAwalData;
            set { SetProperty(ref _hasAwalData, value); }
        }

        #endregion

        #endregion

        #region Delegates

        private DelegateCommand _closeEmployeePreviewCommand = null;
        public DelegateCommand CloseEmployeePreviewCommand => _closeEmployeePreviewCommand ?? (_closeEmployeePreviewCommand = new DelegateCommand(CloseEmployeePreview));

        private DelegateCommand<string> _changeEmployeeStatusCommand = null;
        public DelegateCommand<string> ChangeEmployeeStatusCommand => _changeEmployeeStatusCommand ?? (_changeEmployeeStatusCommand = new DelegateCommand<string>(ChangeEmploymentStatus));

        private DelegateCommand _addSanctionCommand = null;
        public DelegateCommand AddSanctionCommand => _addSanctionCommand ?? (_addSanctionCommand = new DelegateCommand(AddSanction));

        private DelegateCommand _addAwalCommand = null;
        public DelegateCommand AddAwalCommand => _addAwalCommand ?? (_addAwalCommand = new DelegateCommand(AddAwal));

        private DelegateCommand<SanctionEntity?> _onSanctionOverrideCommand = null;
        public DelegateCommand<SanctionEntity?> OnSanctionOverrideCommand => _onSanctionOverrideCommand ?? (_onSanctionOverrideCommand = new DelegateCommand<SanctionEntity?>(OnSanctionOverride));

        private DelegateCommand<SanctionEntity?> _onSanctionReissueCommand = null;
        public DelegateCommand<SanctionEntity?> OnSanctionReissueCommand => _onSanctionReissueCommand ?? (_onSanctionReissueCommand = new DelegateCommand<SanctionEntity?>(OnSanctionReissue));

        #endregion

        private bool _isPageActive;
        private readonly IEventAggregator _eventAggregator;
        private readonly PreviewRepository _previewRepository;

        public EmployeeDataViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _previewRepository = new PreviewRepository();
            _isPageActive = false;
            Avatar = string.Empty;
            SelectedTabIndex = 0;
            IsFileSectionVisible = false;

            SanctionsList = new ObservableCollection<SanctionEntity>();
            AwalList = new ObservableCollection<AwalEntity>();
            Timeline = new ObservableCollection<EmployeeSummary>();

            SanctionList = SanctionManager.GetSanctions();

            AwalNewEntry = new AwalEntry();
        }

        private void GetEmployeeData(string selectedEmployeeId)
        {
            GetHeaders(selectedEmployeeId);
            GetSanctionPreview(selectedEmployeeId);
            GetEmployeeStatus(selectedEmployeeId);
            GetTimeline(selectedEmployeeId);
            GetAllSanctions(selectedEmployeeId);
            GetAwal(selectedEmployeeId);
            GetMeetings(selectedEmployeeId);
            GetCustomMeetings(selectedEmployeeId);
            GetAdapt(selectedEmployeeId);
            GetPersonalLeaveData(selectedEmployeeId);
        }

        #region Data Getters

        private async void GetTimeline(string id)
        {
            WidgedState |= HomePageWidgetState.EmployeeTimelineLoading;
            WidgedState &= ~HomePageWidgetState.EmployeeTimelineLoaded;

            Timeline.Clear();

            Timeline.AddRange(await _previewRepository.GetTimelineAsync(id));
            try
            {
                Timeline.Add(new EmployeeSummary { Date = System.DateTime.Parse(SelectedEmployee.LastHireDate), Event = "AA has been hired" });
            }
            catch { }

            WidgedState &= ~HomePageWidgetState.EmployeeTimelineLoading;
            WidgedState |= HomePageWidgetState.EmployeeTimelineLoaded;
        }

        private async void GetEmployeeStatus(string id)
        {
            WidgedState |= HomePageWidgetState.EmployeeStatusLoading;
            var emplStatus = await _previewRepository.GetEmployeeStatusSnipAsync(id);
            EmplStatus = emplStatus.SuspensionCount > 0 ? EmploymentStatus.Suspended : emplStatus.IsRosterActive ? EmploymentStatus.Active : EmploymentStatus.NotActive;
            WidgedState &= ~HomePageWidgetState.EmployeeStatusLoading;
            WidgedState |= HomePageWidgetState.EmployeeStatusLoaded;
        }

        #region AWAL

        private void AddAwal()
        {
            if (AwalNewEntry.CanAdd())
            {

            }
        }

        private async void GetAwal(string id)
        {
            AwalList.Clear();
            WidgedState &= ~HomePageWidgetState.EmployeeAwalSummaryLoaded;
            WidgedState |= HomePageWidgetState.EmployeeAwalSummaryLoading;

            var awalRepo = new AWALRepository();

            AwalList.AddRange(await awalRepo.GetEmployeeAwalAsync(id));
            HasAwalData = AwalList.Count > 0;

            WidgedState &= ~HomePageWidgetState.EmployeeAwalSummaryLoading;
            WidgedState |= HomePageWidgetState.EmployeeAwalSummaryLoaded;
        }

        #endregion



        private void GetCustomMeetings(string id)
        {

        }

        private void GetMeetings(string id)
        {

        }

        private void GetAdapt(string id)
        {

        }

        private void GetPersonalLeaveData(string id)
        {

        }

        #region Sanctions

        private async void GetAllSanctions(string id)
        {
            SanctionsList.Clear();
            SanctionState &= ~SanctionWidgetState.DataLoaded;
            SanctionState |= SanctionWidgetState.DataLoading;

            var sanctRepo = new SanctionsRepository();
            SanctionsList.AddRange(await sanctRepo.GetEmployeeSanctionsAsync(id));
            HasSanctionData = SanctionsList.Count > 0;

            SanctionState &= ~SanctionWidgetState.DataLoading;
            SanctionState |= SanctionWidgetState.DataLoaded;
        }

        private async void AddSanction()
        {
            if (string.IsNullOrEmpty(SelectedSanction) || SanctionStartDate.Equals(DateTime.MinValue)) return;

            SanctionState &= ~SanctionWidgetState.EditIdle;
            SanctionState |= SanctionWidgetState.EditInProgress;

            var sanction = new SanctionEntity().Init().SetEmployee(SelectedEmployee).SetSanction(SelectedSanction, SanctionStartDate);

            var sRepo = new SanctionsRepository();
            var response = await sRepo.InsertAsync(sanction);
            if (response)
            {
                SanctionsList.Insert(0, sanction);
                HasSanctionData = true;
                GetSanctionPreview(SelectedEmployee.EmployeeID);
                GetTimeline(SelectedEmployee.EmployeeID);
                GetHeaders(SelectedEmployee.EmployeeID);
            }

            SanctionState &= ~SanctionWidgetState.EditInProgress;
            SanctionState |= SanctionWidgetState.EditIdle;

        }

        private async void GetSanctionPreview(string id)
        {
            EmployeeLiveSanctions = await _previewRepository.GetLiveSanctionsPreviewAsync(id);
        }

        private async void OnSanctionOverride(SanctionEntity? sanction)
        {
            if (!sanction.HasValue) return;

            var sanct = sanction.Value;
            if (sanct.Overriden || sanct.SanctionEndDate <= System.DateTime.Now) return;

            var sanctionRepo = new SanctionsRepository();
            var results = await sanctionRepo.OverrideSanctionAsync(sanct);

            try
            {
                SanctionsList.Swap(sanct, results);
                GetSanctionPreview(SelectedEmployee.EmployeeID);
                GetTimeline(SelectedEmployee.EmployeeID);
            }
            catch { }
        }

        private async void OnSanctionReissue(SanctionEntity? sanction)
        {
            if (!sanction.HasValue) return;

            var sanct = sanction.Value;
            if (!sanct.Overriden && sanct.SanctionEndDate <= System.DateTime.Now) return;

            var sanctionRepo = new SanctionsRepository();
            var results = await sanctionRepo.ReissueSanctionAsync(sanct);

            try
            {
                SanctionsList.Swap(sanct, results);
                GetSanctionPreview(SelectedEmployee.EmployeeID);
                GetTimeline(SelectedEmployee.EmployeeID);
            }
            catch { }
        }

        #endregion

        #endregion

        private async void GetHeaders(string emplId)
        {
            PreviewEmplDataSnip = await _previewRepository.GetEmployeePreviewAsync(emplId);
        }

        private async void ChangeEmploymentStatus(string caller)
        {
            WidgedState |= HomePageWidgetState.EmployeeStatusLoading;
            var tempStatus = EmploymentStatus.Default;
            switch (caller)
            {
                case "Activate":
                    tempStatus = EmploymentStatus.Active;
                    break;
                case "Suspend":
                    tempStatus = EmploymentStatus.Suspended;
                    break;
                default:
                    tempStatus = EmploymentStatus.Active;
                    break;
            }

            var successfullyUpdated = await _previewRepository.UpdateEmployeeStatusAsync(tempStatus, SelectedEmployee.EmployeeID);
            if (successfullyUpdated)
            {
                EmplStatus = tempStatus;
                //CacheManager.ResetTimer();
            }

            WidgedState &= ~HomePageWidgetState.EmployeeStatusLoading;
            WidgedState |= HomePageWidgetState.EmployeeStatusLoaded;
        }

        private void CloseEmployeePreview()
        {
            SelectedEmployee = null;
            ClearLists();
            _eventAggregator.GetEvent<NavigationArgs>().Publish("HomePage");
        }

        private void ClearLists()
        {
            HasSanctionData = true;
            SanctionsList.Clear();
            Timeline.Clear();
        }

        private void OnEmployeeSelected(Roster selectedEmployee)
        {
            if (!_isPageActive) return;

            selectedEmployee.SetTenure();
            SelectedEmployee = selectedEmployee;
            Avatar = DataStorage.AppSettings.UserImgURL.Replace("{UserID}", SelectedEmployee.UserID);
            GetEmployeeData(selectedEmployee.EmployeeID);
        }

        #region Navigation

        private void OnTabChange(uint tabIndex)
        {
            switch (tabIndex)
            {
                case 3:
                    IsFileSectionVisible = true;
                    break;
                default:
                    IsFileSectionVisible = false;
                    break;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _isPageActive = false;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _isPageActive = true;
            OnEmployeeSelected(navigationContext.Parameters["selectedEmployee"] as Roster);
            
        }

        #endregion
    }
}
