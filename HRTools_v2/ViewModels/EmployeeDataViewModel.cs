using Domain.DataManager;
using Domain.Extensions;
using Domain.Models;
using Domain.Models.AWAL;
using Domain.Models.DataSnips;
using Domain.Models.Resignations;
using Domain.Models.Sanctions;
using Domain.Repository;
using Domain.States;
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

        private ObservableCollection<Timeline> _timeline;
        public ObservableCollection<Timeline> Timeline
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

        #region Resignation Props

        private ResignationWidgetState _resignationState;
        public ResignationWidgetState ResignationState
        {
            get => _resignationState;
            set { SetProperty(ref _resignationState, value); }
        }

        private List<string> _resignationReasonList;
        public List<string> ResignationReasonList
        {
            get { return _resignationReasonList; }
            set { SetProperty(ref _resignationReasonList, value); }
        }

        private ResignationEntry _resignationNewEntry;
        public ResignationEntry ResignationNewEntry
        {
            get => _resignationNewEntry;
            set { SetProperty(ref _resignationNewEntry, value); }
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

        private DelegateCommand _clearResignationEntryCommand = null;
        public DelegateCommand ClearResignationEntryCommand => _clearResignationEntryCommand ?? (_clearResignationEntryCommand = new DelegateCommand(() => ResignationNewEntry = new ResignationEntry()));

        private DelegateCommand _submitResignationCommand = null;
        public DelegateCommand SubmitResignationCommand => _submitResignationCommand ?? (_submitResignationCommand = new DelegateCommand(SubmitResignation));

        private DelegateCommand _cancelResignationCommand = null;
        public DelegateCommand CancelResignationCommand => _cancelResignationCommand ?? (_cancelResignationCommand = new DelegateCommand(CancelResignation));

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
            Timeline = new ObservableCollection<Timeline>();


            SanctionList = SanctionManager.GetSanctions();

            AwalNewEntry = new AwalEntry();
            ResignationNewEntry = new ResignationEntry();

            SetResignationCategories();
            
        }

        private void SetResignationCategories() => ResignationReasonList = new List<string>
            {
                "Bureaucracy - Ability to Bld",
                "Career Choice Grad",
                "Challenging Work",
                "Compensation",
                "Develop/Career/Promo",
                "End of Assignment",
                "Family Move or Circumstances",
                "Manager/Leadership",
                "Med-unable to perfessent fxn",
                "Mutual Agreement (vol)",
                "Operational Support burden",
                "Recognition for Great Work",
                "Resignation in Lieu of Term",
                "Return to School",
                "Tools and Resources",
                "Work Eligibility",
                "Work Environment",
                "Working Conditions (Facility)"
            };

        private void GetEmployeeData(string selectedEmployeeId)
        {
            GetHeaders(selectedEmployeeId);
            GetSanctionPreview(selectedEmployeeId);
            GetEmployeeStatus(selectedEmployeeId);
            GetResignationData(selectedEmployeeId);
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
                Timeline.Add(new Timeline { CreatedAt = DateTime.Parse(SelectedEmployee.LastHireDate), EventMessage = "AA has been hired" });
            }
            catch (Exception e) 
            {
                LoggerManager.Log("EmployeeDataViewModel.GetTimeline()", e.Message);
            }

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
            if (!AwalNewEntry.CanAdd())
            {
                SendToast("Failed to insert AWAL. Are you sure all the data is correct?", NotificationType.Information);
                return;
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

        #region Resignations


        private async void SubmitResignation()
        {
            ResignationState |= ResignationWidgetState.ResignationSubmitInProgress;


            ResignationState = ResignationWidgetState.ResignationExists;
            ResignationState |= ResignationWidgetState.DataLoading;
`
        }

        private async void CancelResignation()
        {
            ResignationState = ResignationWidgetState.DataLoading;



            ResignationState = ResignationWidgetState.ResignationDoesNotExist;
        }

        private async void GetResignationData(string id)
        {
            ResignationState = ResignationWidgetState.DataLoading;
            var repo = new ResignationsRepository();

            var count = await repo.IsResignedAsync(id);
            if (count > 0) ResignationState = ResignationWidgetState.ResignationExists;
            else ResignationState = ResignationWidgetState.ResignationDoesNotExist;
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
            if (response.Success)
            {
                SendToast("Sanction has been recorded!", NotificationType.Success);
                SanctionsList.Insert(0, sanction);
                HasSanctionData = true;
                GetSanctionPreview(SelectedEmployee.EmployeeID);
                GetTimeline(SelectedEmployee.EmployeeID);
                GetHeaders(SelectedEmployee.EmployeeID);
            }
            else
            {
                SendToast("Failed to insert sancion..", NotificationType.Warning);
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
            if (!sanction.HasValue)
            {
                LoggerManager.Log("EmployeeDataViewModel.OnSanctionOverride", "sanction is NULL");
                return;
            }

            var sanct = sanction.Value;
            if (sanct.Overriden || sanct.SanctionEndDate <= DateTime.Now)
            {
                SendToast("You cannot override this sanction as it already expired!", NotificationType.Information);
                return;
            }

            var sanctionRepo = new SanctionsRepository();
            var results = await sanctionRepo.OverrideSanctionAsync(sanct);

            if (results.OverridenAt.Equals(sanction.Value.OverridenAt))
            {
                SendToast("You cannot override this sanction", NotificationType.Warning);
            }
            else
            {
                SendToast("Sanction has been overriden!", NotificationType.Success);
                try
                {
                    SanctionsList.Swap(sanct, results);
                    GetSanctionPreview(SelectedEmployee.EmployeeID);
                    GetTimeline(SelectedEmployee.EmployeeID);
                }
                catch (Exception e)
                {
                    LoggerManager.Log("EmployeeDataViewModel.OnSanctionOverride", e.Message);
                }
            }
           

            
        }

        private async void OnSanctionReissue(SanctionEntity? sanction)
        {
            if (!sanction.HasValue)
            {
                LoggerManager.Log("EmployeeDataViewModel.OnSanctionOverride", "sanction is NULL");
                return;
            }

            var sanct = sanction.Value;
            if (!sanct.Overriden && sanct.SanctionEndDate <= DateTime.Now)
            {
                SendToast("You cannot re-issue this sanction as it already expired!", NotificationType.Information);
                return;
            }

            var sanctionRepo = new SanctionsRepository();
            var results = await sanctionRepo.ReissueSanctionAsync(sanct);

            if (results.OverridenAt.Equals(sanction.Value.OverridenAt))
            {
                SendToast("You cannot re-issue this sanction", NotificationType.Warning);
            }
            else
            {
                SendToast("Sanction has been re-issued!", NotificationType.Success);
                try
                {
                    SanctionsList.Swap(sanct, results);
                    GetSanctionPreview(SelectedEmployee.EmployeeID);
                    GetTimeline(SelectedEmployee.EmployeeID);
                }
                catch (Exception e)
                {
                    LoggerManager.Log("EmployeeDataViewModel.OnSanctionReissue", e.Message);
                }
            }     
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
            EmploymentStatus tempStatus;
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
            if (successfullyUpdated.Success)
            {
                SendToast("Employment status has been set", NotificationType.Success);
                EmplStatus = tempStatus;
                GetTimeline(SelectedEmployee.EmployeeID);
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

        private void SendToast(string message, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<ShowToastArgs>().Publish((message, notificationType));
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
