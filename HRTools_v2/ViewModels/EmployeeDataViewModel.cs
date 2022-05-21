using Domain.Data;
using Domain.DataManager;
using Domain.Extensions;
using Domain.IO;
using Domain.Models;
using Domain.Models.AWAL;
using Domain.Models.CustomMeetings;
using Domain.Models.DataSnips;
using Domain.Models.Meetings;
using Domain.Models.Resignations;
using Domain.Models.Sanctions;
using Domain.Networking;
using Domain.Repository;
using Domain.States;
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

        #region Meetings

        private ObservableCollection<MeetingsEntity> _meetingsList;
        public ObservableCollection<MeetingsEntity> MeetingsList
        {
            get { return _meetingsList; }
            set { SetProperty(ref _meetingsList, value); }
        }

        private string _reasonForErClosure;
        public string ReasonForErClosure
        {
            get => _reasonForErClosure;
            set { SetProperty(ref _reasonForErClosure, value); }
        }

        private MeetingsEntry _newMeetingCase;
        public MeetingsEntry NewMeetingCase
        {
            get => _newMeetingCase;
            set { SetProperty(ref _newMeetingCase, value); }
        }

        private ObservableCollection<string> _meetingTypes;
        public ObservableCollection<string> MeetingTypes
        {
            get => _meetingTypes;
            set { SetProperty(ref _meetingTypes, value); }
        }


        #endregion

        #region Custom Meetings

        private Roster _claimant;
        public Roster Claimant
        {
            get => _claimant;
            set { SetProperty(ref _claimant, value); }
        }

        private Roster _respondent;
        public Roster Respondent
        {
            get => _respondent;
            set { SetProperty(ref _respondent, value); }
        }

        private ObservableCollection<CustomMeetingEntity> _customMeetingsList;
        public ObservableCollection<CustomMeetingEntity> CustomMeetingsList
        {
            get { return _customMeetingsList; }
            set { SetProperty(ref _customMeetingsList, value); }
        }

        private string _customMeetingType;
        public string CustomMeetingType
        {
            get { return _customMeetingType; }
            set { SetProperty(ref _customMeetingType, value); }
        }

        private string _exactCaseID;
        public string ExactCaseID
        {
            get { return _exactCaseID; }
            set { SetProperty(ref _exactCaseID, value); }
        }

        private List<string> _meetingTypeList;
        public List<string> MeetingTypeList
        {
            get { return _meetingTypeList; }
            set { SetProperty(ref _meetingTypeList, value); }
        }

        private string _reasonForCustomClosure;
        public string ReasonForCustomClosure
        {
            get => _reasonForCustomClosure;
            set { SetProperty(ref _reasonForCustomClosure, value); }
        }

        private ObservableCollection<string> _customMeetingTypes;
        public ObservableCollection<string> CustomMeetingTypes
        {
            get => _customMeetingTypes;
            set { SetProperty(ref _customMeetingTypes, value); }
        }

        #region Search

        private string _claimantSearchText = string.Empty;
        public string ClaimantSearchText
        {
            get => _claimantSearchText;
            set { SetProperty(ref _claimantSearchText, value); HandleClaimantSearch(); }
        }

        private string _respondentSearchText = string.Empty;
        public string RespondentSearchText
        {
            get => _respondentSearchText;
            set { SetProperty(ref _respondentSearchText, value); HandleRespondentSearch(); }
        }

        private ObservableCollection<Roster> _suggestionList;
        public ObservableCollection<Roster> SuggestionList
        {
            get { return _suggestionList; }
            set { SetProperty(ref _suggestionList, value); }
        }

        private UIComponentState _claimantSearchComponentState;
        public UIComponentState ClaimantSearchComponentState
        {
            get => _claimantSearchComponentState;
            set { SetProperty(ref _claimantSearchComponentState, value); }
        }

        private UIComponentState _respondentSearchComponentState;
        public UIComponentState RespondentSearchComponentState
        {
            get => _respondentSearchComponentState;
            set { SetProperty(ref _respondentSearchComponentState, value); }
        }

        #endregion

        #endregion

        #region Awal

        private AwalEntry _awalNewEntry;
        public AwalEntry AwalNewEntry
        {
            get => _awalNewEntry;
            set { SetProperty(ref _awalNewEntry, value); }
        }

        private ObservableCollection<AwalEntity> _awalList;
        public ObservableCollection<AwalEntity> AwalList
        {
            get { return _awalList; }
            set { SetProperty(ref _awalList, value); }
        }

        private bool _isOnAwal;
        public bool IsOnAwal
        {
            get => _isOnAwal;
            set { SetProperty(ref _isOnAwal, value); }
        }

        private List<string> _awalSanctionList;
        public List<string> AwalSanctionList
        {
            get { return _awalSanctionList; }
            set { SetProperty(ref _awalSanctionList, value); }
        }

        #endregion

        #region Timeline

        private ObservableCollection<Timeline> _timeline;
        public ObservableCollection<Timeline> Timeline
        {
            get { return _timeline; }
            set { SetProperty(ref _timeline, value); }
        }

        private TimelineOrigin _timeLineToggleSelection;
        public TimelineOrigin TimeLineToggleSelection
        {
            get => _timeLineToggleSelection;
            set { 
                SetProperty(ref _timeLineToggleSelection, value);
                if (SelectedEmployee != null) GetTimeline(SelectedEmployee.EmployeeID, value);
            }
        }


        #endregion

        #region Comments

        private ObservableCollection<EmplComment> _comments;
        public ObservableCollection<EmplComment> Comments
        {
            get { return _comments; }
            set { SetProperty(ref _comments, value); }
        }

        private string _commentText;
        public string CommentText
        {
            get { return _commentText; }
            set { SetProperty(ref _commentText, value); }
        }

        #endregion

        #region Tasks

        private ObservableCollection<EmplTask> _tasks;
        public ObservableCollection<EmplTask> Tasks
        {
            get { return _tasks; }
            set { SetProperty(ref _tasks, value); }
        }

        private string _taskContent;
        public string TaskContent
        {
            get { return _taskContent; }
            set { SetProperty(ref _taskContent, value); }
        }

        private DateTime _taskDueDate;
        public DateTime TaskDueDate
        {
            get { return _taskDueDate; }
            set { SetProperty(ref _taskDueDate, value); }
        }

        #endregion

        #region Sanction Props

        private ObservableCollection<SanctionEntry> _sanctionsList;
        public ObservableCollection<SanctionEntry> SanctionsList
        {
            get { return _sanctionsList; }
            set { SetProperty(ref _sanctionsList, value); }
        }

        private SanctionWidgetState _sanctionState;
        public SanctionWidgetState SanctionState
        {
            get => _sanctionState;
            set { SetProperty(ref _sanctionState, value); }
        }

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

        private string _resignationTT;

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

        private bool _hasSanctionData;
        public bool HasSanctionData
        {
            get => _hasSanctionData;
            set { SetProperty(ref _hasSanctionData, value); }
        }

        private bool _hasMeetingsData;
        public bool HasMeetingsData
        {
            get => _hasMeetingsData;
            set { SetProperty(ref _hasMeetingsData, value); }
        }

        private bool _hasCustomMeetingsData;
        public bool HasCustomMeetingsData
        {
            get => _hasCustomMeetingsData;
            set { SetProperty(ref _hasCustomMeetingsData, value); }
        }

        private bool _hasCommentsData;
        public bool HasCommentsData
        {
            get => _hasCommentsData;
            set { SetProperty(ref _hasCommentsData, value); }
        }

        private bool _hasTasksData;
        public bool HasTasksData
        {
            get => _hasTasksData;
            set { SetProperty(ref _hasTasksData, value); }
        }

        private bool _hasAwalData;
        public bool HasAwalData
        {
            get => _hasAwalData;
            set { SetProperty(ref _hasAwalData, value); }
        }

        private bool _hasTimelineData;
        public bool HasTimelineData
        {
            get => _hasTimelineData;
            set { SetProperty(ref _hasTimelineData, value); }
        }

        #endregion

        #endregion

        #region Delegates

        #region Empl

        private DelegateCommand _closeEmployeePreviewCommand = null;
        public DelegateCommand CloseEmployeePreviewCommand => _closeEmployeePreviewCommand ?? (_closeEmployeePreviewCommand = new DelegateCommand(CloseEmployeePreview));

        private DelegateCommand _refreshEmployeeDataCommand = null;
        public DelegateCommand RefreshEmployeeDataCommand => _refreshEmployeeDataCommand ?? (_refreshEmployeeDataCommand = new DelegateCommand(RefreshEmployeeData));

        private DelegateCommand<string> _changeEmployeeStatusCommand = null;
        public DelegateCommand<string> ChangeEmployeeStatusCommand => _changeEmployeeStatusCommand ?? (_changeEmployeeStatusCommand = new DelegateCommand<string>(ChangeEmploymentStatus));

        #endregion

        #region Resignation

        private DelegateCommand _clearResignationEntryCommand = null;
        public DelegateCommand ClearResignationEntryCommand => _clearResignationEntryCommand ?? (_clearResignationEntryCommand = new DelegateCommand(() => ResignationNewEntry = new ResignationEntry()));

        private DelegateCommand _submitResignationCommand = null;
        public DelegateCommand SubmitResignationCommand => _submitResignationCommand ?? (_submitResignationCommand = new DelegateCommand(SubmitResignation));

        private DelegateCommand _cancelResignationCommand = null;
        public DelegateCommand CancelResignationCommand => _cancelResignationCommand ?? (_cancelResignationCommand = new DelegateCommand(CancelResignation));

        private DelegateCommand _openResignationTTCommand = null;
        public DelegateCommand OpenResignationTTCommand => _openResignationTTCommand ?? (_openResignationTTCommand = new DelegateCommand(OpenResignationTT));

        private DelegateCommand _openResignationQuicklinkCommand = null;
        public DelegateCommand OpenResignationQuicklinkCommand => _openResignationQuicklinkCommand ?? (_openResignationQuicklinkCommand = new DelegateCommand(OpenResignationQuicklink));

        #endregion

        #region Sanction

        private DelegateCommand _addSanctionCommand = null;
        public DelegateCommand AddSanctionCommand => _addSanctionCommand ?? (_addSanctionCommand = new DelegateCommand(AddSanction));

        private DelegateCommand<SanctionEntry?> _onSanctionOverrideCommand = null;
        public DelegateCommand<SanctionEntry?> OnSanctionOverrideCommand => _onSanctionOverrideCommand ?? (_onSanctionOverrideCommand = new DelegateCommand<SanctionEntry?>(OnSanctionOverride));

        private DelegateCommand<SanctionEntry?> _onSanctionReissueCommand = null;
        public DelegateCommand<SanctionEntry?> OnSanctionReissueCommand => _onSanctionReissueCommand ?? (_onSanctionReissueCommand = new DelegateCommand<SanctionEntry?>(OnSanctionReissue));

        #endregion

        #region AWAL

        private DelegateCommand _addAwalCommand = null;
        public DelegateCommand AddAwalCommand => _addAwalCommand ?? (_addAwalCommand = new DelegateCommand(AddAwal));

        private DelegateCommand<AwalEntity> _onAwalCancelCommand = null;
        public DelegateCommand<AwalEntity> OnAwalCancelCommand => _onAwalCancelCommand ?? (_onAwalCancelCommand = new DelegateCommand<AwalEntity>(CancelAwal));

        private DelegateCommand<AwalEntity> _onAwalEditCommand = null;
        public DelegateCommand<AwalEntity> OnAwalEditCommand => _onAwalEditCommand ?? (_onAwalEditCommand = new DelegateCommand<AwalEntity>(UpdateAwal));

        private DelegateCommand<string> _onAwalLetterRequestedCommand = null;
        public DelegateCommand<string> OnAwalLetterRequestedCommand => _onAwalLetterRequestedCommand ?? (_onAwalLetterRequestedCommand = new DelegateCommand<string>(RequestAwalLetter));

        #endregion

        #region Comments

        private DelegateCommand _addCommentCommand = null;
        public DelegateCommand AddCommentCommand => _addCommentCommand ?? (_addCommentCommand = new DelegateCommand(AddComment));

        private DelegateCommand<EmplComment?> _deleteCommentCommand = null;
        public DelegateCommand<EmplComment?> DeleteCommentCommand => _deleteCommentCommand ?? (_deleteCommentCommand = new DelegateCommand<EmplComment?>(DeleteComment));

        #endregion

        #region Meetings

        private DelegateCommand _onErMeetingCreateCommand = null;
        public DelegateCommand OnErMeetingCreateCommand => _onErMeetingCreateCommand ?? (_onErMeetingCreateCommand = new DelegateCommand(CreateERMeeting));

        private DelegateCommand<MeetingsEntity> _onErMeetingEditCommand = null;
        public DelegateCommand<MeetingsEntity> OnErMeetingEditCommand => _onErMeetingEditCommand ?? (_onErMeetingEditCommand = new DelegateCommand<MeetingsEntity>(EditErMeeting));

        private DelegateCommand<MeetingsEntity> _onMeetingCancelCommand = null;
        public DelegateCommand<MeetingsEntity> OnMeetingCancelCommand => _onMeetingCancelCommand ?? (_onMeetingCancelCommand = new DelegateCommand<MeetingsEntity>(CancelErMeeting));

        private DelegateCommand<MeetingsEntity> _setMeetingPendingCommand = null;
        public DelegateCommand<MeetingsEntity> SetMeetingPendingCommand => _setMeetingPendingCommand ?? (_setMeetingPendingCommand = new DelegateCommand<MeetingsEntity>(SetMeetingPending));

        private DelegateCommand<MeetingsEntity> _reopenMeetingCommand = null;
        public DelegateCommand<MeetingsEntity> ReopenMeetingCommand => _reopenMeetingCommand ?? (_reopenMeetingCommand = new DelegateCommand<MeetingsEntity>(ReopenMeeting));

        #endregion

        #region Custom Meetings

        private DelegateCommand _onCustomMeetingCreateCommand = null;
        public DelegateCommand OnCustomMeetingCreateCommand => _onCustomMeetingCreateCommand ?? (_onCustomMeetingCreateCommand = new DelegateCommand(CreateCustomMeeting));

        private DelegateCommand<CustomMeetingEntity> _onCustomMeetingCancelCommand = null;
        public DelegateCommand<CustomMeetingEntity> OnCustomMeetingCancelCommand => _onCustomMeetingCancelCommand ?? (_onCustomMeetingCancelCommand = new DelegateCommand<CustomMeetingEntity>(CancelCustomMeeting));

        private DelegateCommand<CustomMeetingEntity> _addCaseFileCommand = null;
        public DelegateCommand<CustomMeetingEntity> AddCaseFileCommand => _addCaseFileCommand ?? (_addCaseFileCommand = new DelegateCommand<CustomMeetingEntity>(AddCaseFile));

        private DelegateCommand<CaseFile?> _removeFileCommand = null;
        public DelegateCommand<CaseFile?> RemoveFileCommand => _removeFileCommand ?? (_removeFileCommand = new DelegateCommand<CaseFile?>(RemoveFile));

        private DelegateCommand<CaseFile?> _openFileCommand = null;
        public DelegateCommand<CaseFile?> OpenFileCommand => _openFileCommand ?? (_openFileCommand = new DelegateCommand<CaseFile?>(OpenFile));

        private DelegateCommand<Roster> _onClaimantSelectedCommand = null;
        public DelegateCommand<Roster> OnClaimantSelectedCommand => _onClaimantSelectedCommand ?? (_onClaimantSelectedCommand = new DelegateCommand<Roster>(OnClaimantSelected));

        private DelegateCommand<Roster> _onRespondentSelectedCommand = null;
        public DelegateCommand<Roster> OnRespondentSelectedCommand => _onRespondentSelectedCommand ?? (_onRespondentSelectedCommand = new DelegateCommand<Roster>(OnRespondentSelected));

        private DelegateCommand<string> _assignMeetingParticipantCommand = null;
        public DelegateCommand<string> AssignMeetingParticipantCommand => _assignMeetingParticipantCommand ?? (_assignMeetingParticipantCommand = new DelegateCommand<string>(AssignMeetingParticipant));

        private DelegateCommand<CustomMeetingEntity> _updateCustomMeetingCommand = null;
        public DelegateCommand<CustomMeetingEntity> UpdateCustomMeetingCommand => _updateCustomMeetingCommand ?? (_updateCustomMeetingCommand = new DelegateCommand<CustomMeetingEntity>(UpdateCustomMeeting));
        #endregion

        #endregion

        private bool _isPageActive;
        private readonly IEventAggregator _eventAggregator;
        private readonly PreviewRepository _previewRepository;
        private readonly SearchProvider<Roster> _dataProvider;

        public EmployeeDataViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _previewRepository = new PreviewRepository();
            _dataProvider = new SearchProvider<Roster>();

            ClaimantSearchComponentState = UIComponentState.Hidden;
            RespondentSearchComponentState = UIComponentState.Hidden;

            _isPageActive = false;
            Avatar = string.Empty;
            SelectedTabIndex = 0;
            IsFileSectionVisible = false;
            HasAwalData = true;
            HasTimelineData = true;
            HasSanctionData = true;
            HasCommentsData = true;
            IsOnAwal = false;
            TimeLineToggleSelection = TimelineOrigin.ALL;

            SanctionsList = new ObservableCollection<SanctionEntry>();
            AwalList = new ObservableCollection<AwalEntity>();
            AwalSanctionList = new List<string>();
            Timeline = new ObservableCollection<Timeline>();
            Comments = new ObservableCollection<EmplComment>();
            Tasks = new ObservableCollection<EmplTask>();
            MeetingsList = new ObservableCollection<MeetingsEntity>();
            CustomMeetingsList = new ObservableCollection<CustomMeetingEntity>();
            MeetingTypes = new ObservableCollection<string> { MeetingType.Disciplinary.ToString(), MeetingType.Health.ToString()};
            SuggestionList = new ObservableCollection<Roster>();

            MeetingTypeList = new List<string>
            {
                "", "Adapt", "Time Fraud", "Investigation", "Appeal", "Grievance", "Eligibility", "Formal Probation Review", "TWA"
            };

            SanctionList = SanctionManager.GetSanctions();
            AwalSanctionList = SanctionManager.GetAwalSanctions();

            AwalNewEntry = new AwalEntry();
            ResignationNewEntry = new ResignationEntry();
            NewMeetingCase = new MeetingsEntry();

            SetResignationCategories();
            
        }

        private void RefreshEmployeeData() => GetEmployeeData(SelectedEmployee.EmployeeID);

        private void GetEmployeeData(string selectedEmployeeId)
        {
            GetHeaders(selectedEmployeeId);
            GetSanctionPreview(selectedEmployeeId);
            GetEmployeeStatus(selectedEmployeeId);
            GetResignationData(selectedEmployeeId);
            GetTimeline(selectedEmployeeId, TimeLineToggleSelection);
            GetAllSanctions(selectedEmployeeId);
            GetAwal(selectedEmployeeId);
            GetMeetings(selectedEmployeeId);
            GetCustomMeetings(selectedEmployeeId);
            GetPersonalLeaveData(selectedEmployeeId);
            GetComments(selectedEmployeeId, GetOriginFromTab(SelectedTabIndex));
            GetTasks(selectedEmployeeId, GetOriginFromTab(SelectedTabIndex));
        }

        private async void GetEmployeeStatus(string id)
        {
            WidgedState |= HomePageWidgetState.EmployeeStatusLoading;
            var emplStatus = await _previewRepository.GetEmployeeStatusSnipAsync(id);
            EmplStatus = emplStatus.SuspensionCount > 0 ? EmploymentStatus.Suspended : emplStatus.IsRosterActive ? EmploymentStatus.Active : EmploymentStatus.NotActive;
            WidgedState &= ~HomePageWidgetState.EmployeeStatusLoading;
            WidgedState |= HomePageWidgetState.EmployeeStatusLoaded;
        }

        #region Data Getters

        private async void GetTimeline(string id, TimelineOrigin origin)
        {
            WidgedState |= HomePageWidgetState.EmployeeTimelineLoading;
            WidgedState &= ~HomePageWidgetState.EmployeeTimelineLoaded;

            Timeline.Clear();

            
            Timeline.AddRange(await _previewRepository.GetTimelineAsync(id, origin));
            HasTimelineData = Timeline.Count > 0;


            WidgedState &= ~HomePageWidgetState.EmployeeTimelineLoading;
            WidgedState |= HomePageWidgetState.EmployeeTimelineLoaded;
        }

        #region Supportive Data

        private async void GetComments(string id, TimelineOrigin origin)
        {
            WidgedState |= HomePageWidgetState.EmployeeCommentsLoading;
            WidgedState &= ~HomePageWidgetState.EmployeeCommentsLoaded;

            Comments.Clear();


            Comments.AddRange(await _previewRepository.GetCommentsAsync(id, origin));
            HasCommentsData = Comments.Count > 0;


            WidgedState &= ~HomePageWidgetState.EmployeeCommentsLoading;
            WidgedState |= HomePageWidgetState.EmployeeCommentsLoaded;
        }

        private async void AddComment()
        {
            if (string.IsNullOrEmpty(CommentText))
            {
                SendToast("CAnnot add empty comment", NotificationType.Information);
                return;
            }
            var comment = new EmplComment().Create(CommentText.Trim(), SelectedEmployee.EmployeeID, GetOriginFromTab(SelectedTabIndex));
            var response = await _previewRepository.InsertCommentAsync(comment);

            if (response.Success)
            {
                SendToast("Comment has been added!", NotificationType.Success);
                Comments.Insert(0, comment);
                CommentText = String.Empty;
                HasCommentsData = Comments.Count > 0;
            }
            else
            {
                SendToast(response.Message, NotificationType.Information);
            }
        }

        private async void DeleteComment(EmplComment? comment)
        {
            if (comment == null) return;
            var response = await _previewRepository.DeleteCommentAsync(comment.Value);
            if (response.Success)
            {
                Comments.Remove(comment.Value);
                HasCommentsData = Comments.Count > 0;
                SendToast("Comment has been removed", NotificationType.Success);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
        }

        private async void GetTasks(string id, TimelineOrigin origin)
        {
            WidgedState |= HomePageWidgetState.EmployeeTasksLoading;
            WidgedState &= ~HomePageWidgetState.EmployeeTasksLoaded;

            Tasks.Clear();


            Tasks.AddRange(await _previewRepository.GetTasksAsync(id, origin));
            HasTasksData = Comments.Count > 0;


            WidgedState &= ~HomePageWidgetState.EmployeeTasksLoading;
            WidgedState |= HomePageWidgetState.EmployeeTasksLoaded;
        }

        #endregion

        #region AWAL

        private async void AddAwal()
        {
            var awalRepo = new AWALRepository();
            var awalEntry = new AwalEntity(SelectedEmployee).Add(AwalNewEntry);
            var response = await awalRepo.InsertAsync(awalEntry);
            if (response.Success)
            {
                SendToast("AWAL case has been created!", NotificationType.Success);
                AwalList.Insert(0, awalEntry);
                //GetAwal(SelectedEmployee.EmployeeID);
                HasAwalData = true;

                AwalNewEntry = new AwalEntry();

                IsOnAwal = true;
                if (TimeLineToggleSelection == TimelineOrigin.AWAL || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                GetHeaders(SelectedEmployee.EmployeeID);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
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
            SetIsOnAwal();

            WidgedState &= ~HomePageWidgetState.EmployeeAwalSummaryLoading;
            WidgedState |= HomePageWidgetState.EmployeeAwalSummaryLoaded;
        }

        private void SetIsOnAwal()
        {
            var activeAwal = AwalList.Where(x => x.AwalStatus.Equals(AwalStatus.Active) || x.AwalStatus.Equals(AwalStatus.Pending)).FirstOrDefault();
            IsOnAwal = activeAwal != null;
        }

        private async void CancelAwal(AwalEntity awal)
        {
            if (awal == null|| string.IsNullOrEmpty(awal.ReasonForClosure))
            {
                SendToast("Reason for cancellation is mandatory!", NotificationType.Information);
                return;
            }
            awal.CreateBridge();

            var repo = new AWALRepository();
            var updateResponse = await repo.CloseAsync(awal);
            if (updateResponse.Success)
            {
                AwalList.Swap(awal, awal);
                SendToast("AWAL case has been closed!", NotificationType.Success);
                SetIsOnAwal();
                if (TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimelineOrigin.AWAL);
            }
            else
            {
                SendToast(updateResponse.Message, NotificationType.Warning);
            }
           
        }

        private async void UpdateAwal(AwalEntity awal)
        {
            if (SanctionManager.IsLesser(EmployeeLiveSanctions.DisciplinarySanction, awal.Outcome))
            {
                SendToast("You cannot issue lesser sanction!", NotificationType.Information);
                return;
            }
            var repo = new AWALRepository();
            var response = await repo.UpdateAsync(awal);
            if (!response.Success)
            {
                SendToast(response.Message, NotificationType.Information);
            }
            else
            {
                AwalList.Swap(awal, awal);
                SendToast("AWAL case has been updated!", NotificationType.Success);
                SetIsOnAwal();
                if (!string.IsNullOrEmpty(awal.Outcome)) GetAllSanctions(SelectedEmployee.EmployeeID);
                if (!string.IsNullOrEmpty(awal.Outcome)) GetSanctionPreview(SelectedEmployee.EmployeeID);
                GetHeaders(SelectedEmployee.EmployeeID);
                if (TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimelineOrigin.AWAL);
            }
        }

        private async void RequestAwalLetter(string awalLetterType)
        {
            var repo = new AWALRepository();
            var awalCase = AwalList.Where(x => x.AwalStatus == AwalStatus.Active).FirstOrDefault();
            if (awalCase == null)
            {
                SendToast("No Active AWAL cases", NotificationType.Information);
                return;
            }

            var response = await repo.RequestAwalLetterAsync(awalCase, awalLetterType);
            if (response.Success)
            {
                SendToast($"AWAL {awalLetterType} has been requested!", NotificationType.Success);
                GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
            }
            else
            {
                SendToast(response.Message, NotificationType.Information);
            }
        }

        #endregion

        #region Resignations

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

        private async void SubmitResignation()
        {
            ResignationState |= ResignationWidgetState.ResignationSubmitInProgress;
            var repo = new ResignationsRepository();

            var response = await repo.InsertAsync(new ResignationEntity(ResignationNewEntry).AddEmployee(SelectedEmployee));
            if (response.Success)
            {
                SendToast("Resignation has been submitted!", NotificationType.Success);
                _resignationTT = ResignationNewEntry.TTLink;
                ResignationState = ResignationWidgetState.ResignationExists;
                if (TimeLineToggleSelection == TimelineOrigin.Resignations || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                ResignationNewEntry = new ResignationEntry();
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
                ResignationState = ResignationWidgetState.ResignationDoesNotExist;
            }
        }

        private async void CancelResignation()
        {
            ResignationState = ResignationWidgetState.DataLoading;

            var repo = new ResignationsRepository();

            var response = await repo.CancelResignationAsync(SelectedEmployee.EmployeeID);

            if (response.Success)
            {
                SendToast("Resignation has been cancelled!", NotificationType.Success);
                ResignationState = ResignationWidgetState.ResignationDoesNotExist;
                if (TimeLineToggleSelection == TimelineOrigin.Resignations || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                GetAwal(SelectedEmployee.EmployeeID);
                GetMeetings(SelectedEmployee.EmployeeID);
            }
            else
            {
                SendToast("Failed to cancel resignation", NotificationType.Warning);
                ResignationState = ResignationWidgetState.ResignationExists;
            }
        }

        private async void GetResignationData(string id)
        {
            ResignationState = ResignationWidgetState.DataLoading;
            var repo = new ResignationsRepository();

            var resignationTT = await repo.IsResignedAsync(id);
            if (!string.IsNullOrEmpty(resignationTT))
            {
                ResignationState = ResignationWidgetState.ResignationExists;
                _resignationTT = resignationTT;
            }
            else ResignationState = ResignationWidgetState.ResignationDoesNotExist;
        }

        private void OpenResignationQuicklink()
        {
            WebHelper.OpenLink(DataStorage.AppSettings.ResignationsQuicklinkURL);
        }

        private void OpenResignationTT()
        {
            WebHelper.OpenLink(_resignationTT);
        }

        #endregion

        #region Meetings

        private async void GetMeetings(string id)
        {
            MeetingsList.Clear();
            WidgedState &= ~HomePageWidgetState.EmployeeERMeetingsLoaded;
            WidgedState |= HomePageWidgetState.EmployeeERMeetingsLoading;

            var repo = new MeetingsRepository();
            var list = await repo.GetEmployeeMeetingsAsync(id);
            foreach (var item in list)
            {
                item.SetProgress();
            }

            MeetingsList.AddRange(list);
            HasMeetingsData = MeetingsList.Count > 0;

            WidgedState &= ~HomePageWidgetState.EmployeeERMeetingsLoading;
            WidgedState |= HomePageWidgetState.EmployeeERMeetingsLoaded;
        }

        private async void EditErMeeting(MeetingsEntity meeting)
        {
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
                if (!string.IsNullOrEmpty(meeting.SecondMeetingOutcome))
                {
                    GetSanctionPreview(SelectedEmployee.EmployeeID);
                    GetAllSanctions(SelectedEmployee.EmployeeID);
                }
                if (TimeLineToggleSelection == TimelineOrigin.Meetings || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                MeetingsList.Swap(meeting, meeting);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
        }

        private async void SetMeetingPending (MeetingsEntity meeting)
        {
            var repo = new MeetingsRepository();
            var response = await repo.ChangeMeetingStatusAsync(meeting, "Pending");

            if (response.Success)
            {
                SendToast("Meeting status has been updated!", NotificationType.Success);
                if (TimeLineToggleSelection == TimelineOrigin.Meetings || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                MeetingsList.Swap(meeting, meeting);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
        }

        private async void ReopenMeeting (MeetingsEntity meeting)
        {
            var repo = new MeetingsRepository();
            var response = await repo.ChangeMeetingStatusAsync(meeting, "Open");

            if (response.Success)
            {
                SendToast("Meeting status has been updated!", NotificationType.Success);
                if (TimeLineToggleSelection == TimelineOrigin.Meetings || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                MeetingsList.Swap(meeting, meeting);
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
                if (TimeLineToggleSelection == TimelineOrigin.Meetings || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                ReasonForErClosure = string.Empty;
                MeetingsList.Swap(meeting, meeting);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }

        }

        private async void CreateERMeeting()
        {
            if (!NewMeetingCase.CanAdd())
            {
                SendToast("All fields are mandatory!", NotificationType.Information);
                return;
            }

            if (!NewMeetingCase.ID.IsValidDigitID())
            {
                SendToast("Please enter valid case ID! Or, did you mean to create custom case?", NotificationType.Information);
                return;
            }

            var newMeeting = new MeetingsEntity(NewMeetingCase, SelectedEmployee);
            var meetingRepo = new MeetingsRepository();
            var response = await meetingRepo.InsertAsync(newMeeting);

            if (response.Success)
            {
                SendToast("Meeting has been created!", NotificationType.Success);
                MeetingsList.Insert(0, newMeeting);
                HasMeetingsData = MeetingsList.Count > 0;
                if (TimeLineToggleSelection == TimelineOrigin.Meetings || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                GetHeaders(SelectedEmployee.EmployeeID);
                NewMeetingCase = new MeetingsEntry();
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
        }

        #endregion

        #region Custom Meetings

        private async void CreateCustomMeeting()
        {
            if (string.IsNullOrEmpty(CustomMeetingType))
            {
                SendToast("Please select meeting type", NotificationType.Information);
                return;
            }

            if (Claimant == null && Respondent == null)
            {
                SendToast("Please select Claimant and/or Respondent", NotificationType.Information);
                return;
            }

            var meeting = new CustomMeetingEntity(CustomMeetingType, ExactCaseID).SetClaimant(Claimant).SetRespondent(Respondent);

            var meetingRepo = new MeetingsRepository();
            var response = await meetingRepo.InsertCustomAsync(meeting);

            if (response.Success)
            {
                SendToast("Meeting has been created!", NotificationType.Success);
                meeting.Prepare(SelectedEmployee.EmployeeID);
                CustomMeetingsList.Insert(0, meeting);
                HasCustomMeetingsData = CustomMeetingsList.Count > 0;
                if (TimeLineToggleSelection == TimelineOrigin.CustomMeetings || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                GetHeaders(SelectedEmployee.EmployeeID);
                ClearCreateCustomMeetingSelections();

                var path = Environment.UserName == "eslut" ? $"{DataStorage.AppSettings.MeetingContentTestPath}\\{meeting.ID}" : $"{DataStorage.AppSettings.MeetingContentProductionPath}\\{meeting.ID}";
                FileHelper.CreateDirectoryIfNotExists(path);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
           
        }

        private void ClearCreateCustomMeetingSelections()
        {
            CustomMeetingType = null;
            Claimant = null;
            Respondent = null;
            ExactCaseID = null;
        }

        private async void GetCustomMeetings(string id)
        {
            CustomMeetingsList.Clear();
            WidgedState &= ~HomePageWidgetState.EmployeeCustomMeetingsLoaded;
            WidgedState |= HomePageWidgetState.EmployeeCustomMeetingsLoading;

            var repo = new MeetingsRepository();
            var list = await repo.GetEmployeeCustomMeetingsAsync(id);
            foreach (var item in list)
            {
                item.Prepare(SelectedEmployee.EmployeeID);
            }

            CustomMeetingsList.AddRange(list);
            HasCustomMeetingsData = CustomMeetingsList.Count > 0;

            WidgedState &= ~HomePageWidgetState.EmployeeCustomMeetingsLoading;
            WidgedState |= HomePageWidgetState.EmployeeCustomMeetingsLoaded;
        }

        private async void CancelCustomMeeting(CustomMeetingEntity meeting)
        {
            if (string.IsNullOrEmpty(ReasonForCustomClosure))
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
            var response = await meetingRepo.CloseCustomMeeting(meeting, ReasonForCustomClosure);
            if (response.Success)
            {
                SendToast("Meeting has been closed!", NotificationType.Success);
                if (TimeLineToggleSelection == TimelineOrigin.CustomMeetings || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                ReasonForErClosure = string.Empty;
                CustomMeetingsList.Swap(meeting, meeting);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }

        }

        private async void AddCaseFile(CustomMeetingEntity meeting)
        {
            var dialog = new DialogHelper(".*", "");
            var path = dialog.ShowOpenDialog();
            if (string.IsNullOrEmpty(path)) return;

            var response = await FileHelper.CopyFileToMeetingIDAsync(path, meeting.ID);
            if (response.Success)
            {
                meeting.SetFiles();
            }
        }

        private void RemoveFile (CaseFile? file)
        {
            if (file == null) return;

            var meetingID = FileHelper.GetParentName(file.Value.Path);
            if (meetingID == null) return;

            FileHelper.Delete(file.Value.Path);
            var meeting = CustomMeetingsList.Where(x => x.ID == meetingID).FirstOrDefault();
            if (meeting == null) return;

            SendToast("File has been removed", NotificationType.Success);
            meeting.SetFiles();
            CustomMeetingsList.Swap(meeting, meeting);
        }

        private void OpenFile(CaseFile? file)
        {
            if (file == null) return;
            FileHelper.RunProcess(file.Value.Path);
        }

        private void OnClaimantSelected(Roster empl)
        {
            ClaimantSearchComponentState = UIComponentState.Hidden;
            SuggestionList.Clear();

            Claimant = empl;
            if (Respondent == Claimant) Respondent = null;
        }

        private void OnRespondentSelected(Roster empl)
        {
            RespondentSearchComponentState = UIComponentState.Hidden;
            SuggestionList.Clear();

            Respondent = empl;
            if (Respondent == Claimant) Claimant = null;
        }

        public async void HandleClaimantSearch()
        {
            SuggestionList.Clear();

            if (string.IsNullOrEmpty(ClaimantSearchText))
            {
                ClaimantSearchComponentState = UIComponentState.Hidden;
                Claimant = null;
                return;
            }

            ClaimantSearchComponentState = UIComponentState.Loading;

            if (_dataProvider.IsSearching)
            {
                _dataProvider.SetValues(ClaimantSearchText);
                return;
            }

            _dataProvider.SetValues(ClaimantSearchText, DataStorage.RosterList);
            var result = await _dataProvider.LookUpAsync();
            
            UpdateList(result, ClaimantSearchText);
            ClaimantSearchComponentState = result.Count > 0 ? UIComponentState.Visible : UIComponentState.Empty;

        }

        public async void HandleRespondentSearch()
        {
            SuggestionList.Clear();

            if (string.IsNullOrEmpty(RespondentSearchText))
            {
                RespondentSearchComponentState = UIComponentState.Hidden;
                Respondent = null;
                return;
            }

            RespondentSearchComponentState = UIComponentState.Loading;

            if (_dataProvider.IsSearching)
            {
                _dataProvider.SetValues(RespondentSearchText);
                return;
            }

            _dataProvider.SetValues(RespondentSearchText, DataStorage.RosterList);
            var result = await _dataProvider.LookUpAsync();

            UpdateList(result, RespondentSearchText);
            RespondentSearchComponentState = result.Count > 0 ? UIComponentState.Visible : UIComponentState.Empty;
        }

        private void UpdateList(List<Roster> list, string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                if (list.Count > 50)
                {
                    SuggestionList.AddRange(list.GetRange(0, 49));
                }
                else
                {
                    SuggestionList.AddRange(list);
                }
            }
        }

        private void AssignMeetingParticipant(string type)
        {
            if (string.IsNullOrEmpty(type)) return;

            switch(type)
            {
                case "Claimant":
                    Claimant = SelectedEmployee;
                    if (Respondent == Claimant) Respondent = null;
                    break;
                case "Respondent":
                    Respondent = SelectedEmployee;
                    if (Respondent == Claimant) Claimant = null;
                    break;
                default:
                    break;
            }
        }

        private async void UpdateCustomMeeting(CustomMeetingEntity meeting)
        {
            if (meeting == null) return;

            if (SanctionManager.IsLesser(EmployeeLiveSanctions.DisciplinarySanction, meeting.SecondMeetingOutcome))
            {
                SendToast("You cannot issue lesser sanction!", NotificationType.Information);
                return;
            }

            var repo = new MeetingsRepository();
            var response = await repo.UpdateCustomAsync(meeting);
            if (response.Success)
            {
                SendToast("Meeting has been updated!", NotificationType.Success);
                if (!string.IsNullOrEmpty(meeting.SecondMeetingOutcome))
                {
                    GetSanctionPreview(SelectedEmployee.EmployeeID);
                    GetAllSanctions(SelectedEmployee.EmployeeID);
                }
                if (TimeLineToggleSelection == TimelineOrigin.CustomMeetings || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                CustomMeetingsList.Swap(meeting, meeting);
            }
            else
            {
                SendToast(response.Message, NotificationType.Warning);
            }
            
        }

        #endregion

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

            var sanction = new SanctionEntry().Init().SetEmployee(SelectedEmployee).SetSanction(SelectedSanction, SanctionStartDate);

            var sRepo = new SanctionsRepository();
            var response = await sRepo.InsertAsync(sanction);
            if (response.Success)
            {
                SendToast("Sanction has been recorded!", NotificationType.Success);
                SanctionsList.Insert(0, sanction);
                HasSanctionData = true;
                GetSanctionPreview(SelectedEmployee.EmployeeID);
                if (TimeLineToggleSelection == TimelineOrigin.Sanctions || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
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

        private async void OnSanctionOverride(SanctionEntry? sanction)
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
                    if (TimeLineToggleSelection == TimelineOrigin.Sanctions || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
                }
                catch (Exception e)
                {
                    LoggerManager.Log("EmployeeDataViewModel.OnSanctionOverride", e.Message);
                }
            }
           

            
        }

        private async void OnSanctionReissue(SanctionEntry? sanction)
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
                    if (TimeLineToggleSelection == TimelineOrigin.Sanctions || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
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
                if (TimeLineToggleSelection == TimelineOrigin.Suspensions || TimeLineToggleSelection == TimelineOrigin.ALL) GetTimeline(SelectedEmployee.EmployeeID, TimeLineToggleSelection);
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
            HasMeetingsData = true;
            HasAwalData = true;
            MeetingsList.Clear();
            AwalList.Clear();
            SanctionsList.Clear();
            Timeline.Clear();
        }

        private void OnEmployeeSelected(Roster selectedEmployee)
        {
            if (!_isPageActive) return;

            ClearCreateCustomMeetingSelections();
            ClearLists();
            SelectedEmployee = selectedEmployee;
            Avatar = DataStorage.AppSettings.UserImgURL.Replace("{userID}", SelectedEmployee.UserID);
            GetEmployeeData(selectedEmployee.EmployeeID);
            
        }

        private void SendToast(string message, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<ShowToastArgs>().Publish((message, notificationType));
        }

        #region Navigation

        private void OnTabChange(uint tabIndex)
        {
            if (SelectedEmployee == null) return;
            GetComments(SelectedEmployee.EmployeeID, GetOriginFromTab(tabIndex));
        }

        private TimelineOrigin GetOriginFromTab(uint tabIndex)
        {
            switch (tabIndex)
            {
                case 0:
                    return TimelineOrigin.ALL;
                    case 1:
                    return TimelineOrigin.Sanctions;
                    case 2:
                    return TimelineOrigin.Meetings;
                    case 3:
                    return TimelineOrigin.CustomMeetings;
                case 4:
                    return TimelineOrigin.AWAL;
                default:
                    return TimelineOrigin.ALL;
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
