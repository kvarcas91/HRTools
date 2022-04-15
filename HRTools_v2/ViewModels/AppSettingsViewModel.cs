﻿using Domain.Factory;
using Domain.IO;
using Domain.Models;
using Domain.Models.AWAL;
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

namespace HRTools_v2.ViewModels
{
    public class AppSettingsViewModel : BindableBase, INavigationAware
    {

        private Settings _appSettings;
        public Settings AppSettings
        {
            get => _appSettings;
            set { SetProperty(ref _appSettings, value); }
        }

        #region Data Import Properties

        private AwalImportMap _awalMap;
        public AwalImportMap AwalMap
        {
            get => _awalMap;
            set { SetProperty(ref _awalMap, value); }
        }

        private MeetingsImportMap _meetingsMap;
        public MeetingsImportMap MeetingsMap
        {
            get => _meetingsMap;
            set { SetProperty(ref _meetingsMap, value); }
        }

        #region Awal

        private bool _isAwalDataImportInProgress;
        public bool IsAwalDataImportInProgress
        {
            get => _isAwalDataImportInProgress;
            set { SetProperty(ref _isAwalDataImportInProgress, value); }
        }

        private string _awalFileName;
        public string AwalFileName
        {
            get => _awalFileName;
            set { SetProperty(ref _awalFileName, value); }
        }

        #endregion

        #region Meetings

        private bool _isMeetingsDataImportInProgress;
        public bool IsMeetingsDataImportInProgress
        {
            get => _isMeetingsDataImportInProgress;
            set { SetProperty(ref _isMeetingsDataImportInProgress, value); }
        }

        private string _meetingsFileName;
        public string MeetingsFileName
        {
            get => _meetingsFileName;
            set { SetProperty(ref _meetingsFileName, value); }
        }

        #endregion


        #endregion

        #region Delegates

        #region Awal

        private DelegateCommand _importAwalFileCommand = null;
        public DelegateCommand ImportAwalFileCommand => _importAwalFileCommand ?? (_importAwalFileCommand = new DelegateCommand(ImportAwal));

        private DelegateCommand _openAwalFileCommand = null;
        public DelegateCommand OpenAwalFileCommand => _openAwalFileCommand ?? (_openAwalFileCommand = new DelegateCommand(OpenAwalFile));

        #endregion

        #region Meetings

        private DelegateCommand _importMeetingsFileCommand = null;
        public DelegateCommand ImportMeetingsFileCommand => _importMeetingsFileCommand ?? (_importMeetingsFileCommand = new DelegateCommand(ImportMeetings));

        private DelegateCommand _openMeetingsFileCommand = null;
        public DelegateCommand OpenMeetingsFileCommand => _openMeetingsFileCommand ?? (_openMeetingsFileCommand = new DelegateCommand(OpenMeetingsFile));

        #endregion

        #endregion

        private readonly IEventAggregator _eventAggregator;

        public AppSettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            AwalMap = new AwalImportMap().SetDefaultValues();
            MeetingsMap = new MeetingsImportMap().SetDefaultValues();
            IsAwalDataImportInProgress = false;
        }

        #region Awal

        private async void ImportAwal()
        {
            if (string.IsNullOrEmpty(AwalFileName))
            {
                SendMessage("Import AWAL data as .csv file first", NotificationType.Information);
                return;
            }
            IsAwalDataImportInProgress = true;
            var dataMap = new DataMap(AwalMap, DataImportType.Awal);
            var csvReader = new CSVStream(AwalFileName);
            var csvOutput = csvReader.Get(dataMap);
            var awalRepo = new AWALRepository();
            var response = await awalRepo.InsertAllAsync(csvOutput);
            IsAwalDataImportInProgress = false;

            if (response.Success)
            {
                SendMessage("AWAL data has been imported!", NotificationType.Success);
            }
            else SendMessage(response.Message, NotificationType.Warning);


        }

        private void OpenAwalFile()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv");
            var path = dialog.ShowOpenDialog();
            if (string.IsNullOrEmpty(path)) return;

            AwalFileName = FileHelper.FileFullName(path);
        }

        #endregion

        #region Meetings

        private async void ImportMeetings()
        {
            if (string.IsNullOrEmpty(MeetingsFileName))
            {
                SendMessage("Import Meetings data as .csv file first", NotificationType.Information);
                return;
            }
            IsMeetingsDataImportInProgress = true;
            var dataMap = new DataMap(MeetingsMap, DataImportType.Meetings);
            var csvReader = new CSVStream(MeetingsFileName);
            var csvOutput = csvReader.Get(dataMap);
            var awalRepo = new MeetingsRepository();
            var response = await awalRepo.InsertAllAsync(csvOutput);
            IsMeetingsDataImportInProgress = false;

            if (response.Success)
            {
                SendMessage("Meetings data has been imported!", NotificationType.Success);
            }
            else SendMessage(response.Message, NotificationType.Warning);


        }

        private void OpenMeetingsFile()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv");
            var path = dialog.ShowOpenDialog();
            if (string.IsNullOrEmpty(path)) return;

            MeetingsFileName = FileHelper.FileFullName(path);
        }

        #endregion


        private void SendMessage(string message, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<ShowToastArgs>().Publish((message, notificationType));
        }

        #region Navigation

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            AppSettings = DataStorage.AppSettings;
        }

        #endregion

    }
}
