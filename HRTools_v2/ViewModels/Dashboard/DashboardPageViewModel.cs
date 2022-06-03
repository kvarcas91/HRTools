using Domain.Data;
using Domain.Helpers;
using Domain.Interfaces;
using Domain.IO;
using Domain.Models.DataSnips;
using Domain.Repository;
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
using System.Globalization;

namespace HRTools_v2.ViewModels.Dashboard
{
    public class DashboardPageViewModel : BindableBase, INavigationAware
    {

        private bool _isPageActive;
        private uint _selectedTabIndex;
        public uint SelectedTabIndex
        {
            get => _selectedTabIndex;
            set { SetProperty(ref _selectedTabIndex, value); OnTabChange(); }
        }

        #region Loader

        private bool _isMetricsLoading = true;
        public bool IsMetricsLoading
        {
            get => _isMetricsLoading;
            set { SetProperty(ref _isMetricsLoading, value); }
        }

        #endregion

        private int _year = -1;
        public int Year
        {
            get => _year;
            set { SetProperty(ref _year, value); OnYearChange(); }
        }

        private int _weekNumber = -1;
        public int WeekNumber
        {
            get => _weekNumber;
            set { SetProperty(ref _weekNumber, value); GetData(); }
        }

        private ObservableCollection<int> _yearList;
        public ObservableCollection<int> YearList
        {
            get => _yearList;
            set { SetProperty(ref _yearList, value); }
        }

        private ObservableCollection<int> _weekList;
        public ObservableCollection<int> WeekList
        {
            get => _weekList;
            set { SetProperty(ref _weekList, value); }
        }

        #region Metrics

        private MeetingMetrics _meetingMetrics;
        public MeetingMetrics MeetingMetrics
        {
            get => _meetingMetrics;
            set { SetProperty(ref _meetingMetrics, value); }
        }

        private CustomMeetingMetrics _customMeetingMetrics;
        public CustomMeetingMetrics CustomMeetingMetrics
        {
            get => _customMeetingMetrics;
            set { SetProperty(ref _customMeetingMetrics, value); }
        }

        private AwalMetrics _awalMetrics;
        public AwalMetrics AwalMetrics
        {
            get => _awalMetrics;
            set { SetProperty(ref _awalMetrics, value); }
        }

        #endregion

        private DelegateCommand _exportCurrentSelectionCommand = null;
        public DelegateCommand ExportCurrentSelectionCommand => _exportCurrentSelectionCommand ?? (_exportCurrentSelectionCommand = new DelegateCommand(ExportData));
        
        private DelegateCommand<string> _changeWeekCommand = null;
        public DelegateCommand<string> ChangeWeekCommand => _changeWeekCommand ?? (_changeWeekCommand = new DelegateCommand<string>(OnChangeWeek));

        private readonly DashboardRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        public DashboardPageViewModel(IEventAggregator eventAggregator)
        {
            _repository = new DashboardRepository();
            _eventAggregator = eventAggregator;
            _selectedTabIndex = 0;
            _year = DateTime.Now.AddDays(-7).Year;
            _weekNumber = GetWeek(-7);
            SetWeeks();
            SetDropDownList();
        }

        private void SetWeeks()
        {
            WeekList = new ObservableCollection<int>();
            for (int i = 52; i > 0; i--)
            {
                WeekList.Add(i);
            }
        }

        private async void SetDropDownList()
        {
            YearList = new ObservableCollection<int>();
            try
            {
                YearList.AddRange(await _repository.GetDistinctYears(SelectedTabIndex));
            }
            catch (ArgumentException e)
            {
                SendToast("Failed to identify database table",NotificationType.Error);
            }
            
        }

        private void OnYearChange()
        {

            WeekNumber = Year == DateTime.Now.Year ? GetWeek() : WeekList[WeekList.Count - 1];
        }

        private int GetWeek(int days = 0) => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(days), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);

        private void SendToast(string message, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<ShowToastArgs>().Publish((message, notificationType));
        }

        private void OnChangeWeek(string action)
        {
            if (action.Equals("-1")) SetOneWeekBackward();
            else SetOneWeekForward();
        }

        private void SetOneWeekBackward()
        {
            var currentWeekIndex = WeekList.IndexOf(WeekNumber);
            if (currentWeekIndex != WeekList.Count - 1) WeekNumber = WeekList[currentWeekIndex + 1];
            else
            {
                var currentYearIndex = YearList.IndexOf(Year);
                if (currentYearIndex + 1 >= YearList.Count) return;

                Year = YearList[currentYearIndex + 1];
            }
        }

        private void SetOneWeekForward()
        {
            var currentWeekIndex = WeekList.IndexOf(WeekNumber);
            if (currentWeekIndex != 0) WeekNumber = WeekList[currentWeekIndex - 1];
            else
            {
                var currentYearIndex = YearList.IndexOf(Year);
                if (currentYearIndex == 0) return;

                Year = YearList[currentYearIndex - 1];
            }
        }

        private async void GetData()
        {
            IsMetricsLoading = true;
            switch (SelectedTabIndex)
            {
                case 0:
                    var metric =  await _repository.GetMeetingMetrics(DateHelper.FirstDateOfWeekISO8601(Year, WeekNumber));
                    if (metric != null) metric.SetRatios();
                    MeetingMetrics = metric;
                    break;
                case 1:
                    var customMetric = await _repository.GetCustomMeetingMetrics(DateHelper.FirstDateOfWeekISO8601(Year, WeekNumber));
                    if (customMetric != null) customMetric.SetRatios();
                    CustomMeetingMetrics = customMetric;
                    break;
                case 2:
                    var awalMetric = await _repository.GetAwalMetrics(DateHelper.FirstDateOfWeekISO8601(Year, WeekNumber));
                    AwalMetrics = awalMetric;
                    break;
                default: break;
            }
            IsMetricsLoading = false;
        }

        private async void ExportData()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            switch (SelectedTabIndex)
            {
                case 0:
                    var list = await _repository.GetSelectedMeetingsAsync(DateHelper.FirstDateOfWeekISO8601(Year, WeekNumber));
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            item.SetProgress();
                        }
                    }
                    Export(path, list);
                    break;
                case 1:
                    var customList = await _repository.GetSelectedCustomMeetingsAsync(DateHelper.FirstDateOfWeekISO8601(Year, WeekNumber));
                    if (customList != null)
                    {
                        foreach (var item in customList)
                        {
                            item.Prepare();
                        }
                    }
                    Export(path, customList);
                    break;
                case 2:
                    var awalList = await _repository.GetSelectedAwalAsync(DateHelper.FirstDateOfWeekISO8601(Year, WeekNumber));
                    Export(path, awalList);
                    break;
                default: break;
            }
            
        }

        private async void Export(string path, IEnumerable<IWritable> list)
        {
            var csvStream = new CSVStream(path);
            var dataManager = new DataManager();

            await dataManager.WriteToCsvAsync(csvStream, list);

            FileHelper.RunProcess(path);
        }

        #region Navigation

        private void OnTabChange()
        {
            if (!_isPageActive) return;

            GetData();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _isPageActive = false;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _isPageActive = true;
            GetData();
        }

        #endregion

    }
}
