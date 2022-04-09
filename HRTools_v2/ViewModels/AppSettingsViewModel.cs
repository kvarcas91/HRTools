using Domain.IO;
using Domain.Models;
using Domain.Models.AWAL;
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

        #region Delegates

        private DelegateCommand _importAwalFileCommand = null;
        public DelegateCommand ImportAwalFileCommand => _importAwalFileCommand ?? (_importAwalFileCommand = new DelegateCommand(ImportAwal));

        private DelegateCommand _openAwalFileCommand = null;
        public DelegateCommand OpenAwalFileCommand => _openAwalFileCommand ?? (_openAwalFileCommand = new DelegateCommand(OpenAwalFile));

        #endregion

        private readonly IEventAggregator _eventAggregator;

        public AppSettingsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            AwalMap = new AwalImportMap().SetDefaultValues();
            IsAwalDataImportInProgress = false;
        }

        private void ImportAwal()
        {
            if (string.IsNullOrEmpty(AwalFileName))
            {
                SendMessage("Import AWAL data as .csv file first", NotificationType.Information);
                return;
            }
            IsAwalDataImportInProgress = true;


        }

        private void OpenAwalFile()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv");
            var path = dialog.ShowOpenDialog();
            if (string.IsNullOrEmpty(path)) return;

            AwalFileName = FileHelper.FileName(path);
        }

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
