using Domain.Models;
using Domain.Models.AWAL;
using Domain.Storage;
using Prism.Commands;
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

        #endregion

        #region Delegates

        private DelegateCommand _importAwalFileCommand = null;
        public DelegateCommand ImportAwalFileCommand => _importAwalFileCommand ?? (_importAwalFileCommand = new DelegateCommand(ImportAwal));

        #endregion

        public AppSettingsViewModel()
        {
            AwalMap = new AwalImportMap().SetDefaultValues();
        }

        private void ImportAwal()
        {

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
