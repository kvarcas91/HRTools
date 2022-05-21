using Domain.Data;
using Domain.IO;
using Domain.Models;
using Domain.Models.Sanctions;
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

namespace HRTools_v2.ViewModels.Sanctions
{
    public class SanctionsPageViewModel : BindableBase, INavigationAware
    {
        private bool _isCurrentPage;
        private readonly IEventAggregator _eventAggregator;
        private readonly SanctionsRepository _repository;

        private bool _showAllSanctions;
        public bool ShowAllSanctions
        {
            get => _showAllSanctions;
            set { SetProperty(ref _showAllSanctions, value); GetData(); }
        }

        private HomePageWidgetState _widgedState;
        public HomePageWidgetState WidgedState
        {
            get => _widgedState;
            set { SetProperty(ref _widgedState, value); }
        }

        private ObservableCollection<SanctionEntry> _sanctionsList;
        public ObservableCollection<SanctionEntry> SanctionsList
        {
            get { return _sanctionsList; }
            set { SetProperty(ref _sanctionsList, value); }
        }

        private bool _hasData;
        public bool HasData
        {
            get => _hasData;
            set { SetProperty(ref _hasData, value); }
        }

        private DelegateCommand<SanctionEntry?> _openEmployeeViewCommand = null;
        public DelegateCommand<SanctionEntry?> OpenEmployeeViewCommand => _openEmployeeViewCommand ?? (_openEmployeeViewCommand = new DelegateCommand<SanctionEntry?>(OpenEmployeeView));

        private DelegateCommand _exportSanctionsCommand = null;
        public DelegateCommand ExportSanctionsCommand => _exportSanctionsCommand ?? (_exportSanctionsCommand = new DelegateCommand(ExportData));

        public SanctionsPageViewModel(IEventAggregator eventAggregator)
        {
            ShowAllSanctions = false;
            HasData = true;
            WidgedState = HomePageWidgetState.SummaryLoading;
            _eventAggregator = eventAggregator;
            _repository = new SanctionsRepository();
            SanctionsList = new ObservableCollection<SanctionEntry>();
        }

        private async void GetData()
        {
            if (!_isCurrentPage) return;

            WidgedState = HomePageWidgetState.SummaryLoading;

            SanctionsList.AddRange(await _repository.GetAllAsync(ShowAllSanctions));
            HasData = SanctionsList.Count > 0;

            WidgedState = HomePageWidgetState.SummaryLoaded;
        }

        private void OpenEmployeeView(SanctionEntry? entity)
        {
            if (entity == null) return;
            var empl = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(entity.Value.EmployeeID)).FirstOrDefault();
            if (empl == null)
            {
                empl = new Roster(entity.Value);
            }

            _eventAggregator.GetEvent<NavigationEmplArgs>().Publish(("EmployeeData", empl));
        }

        private async void ExportData()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save sanctions data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            var csvStream = new CSVStream(path);
            var dataManager = new DataManager();
            await dataManager.WriteToCsvAsync(csvStream, SanctionsList);

            FileHelper.RunProcess(path);
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
            GetData();
        }

        #endregion
    }
}
