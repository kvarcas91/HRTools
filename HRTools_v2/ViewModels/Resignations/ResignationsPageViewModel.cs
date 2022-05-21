using Domain.Data;
using Domain.IO;
using Domain.Models;
using Domain.Models.Resignations;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using HRTools_v2.Args;
using HRTools_v2.Helpers;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;

namespace HRTools_v2.ViewModels.Resignations
{
    public class ResignationsPageViewModel : BindableBase, INavigationAware
    {

        private bool _isCurrentPage;
        private readonly ResignationsRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        private HomePageWidgetState _widgedState;
        public HomePageWidgetState WidgedState
        {
            get => _widgedState;
            set { SetProperty(ref _widgedState, value); }
        }

        private ObservableCollection<ResignationEntity> _resignationsList;
        public ObservableCollection<ResignationEntity> ResignationsList
        {
            get { return _resignationsList; }
            set { SetProperty(ref _resignationsList, value); }
        }

        private bool _hasData;
        public bool HasData
        {
            get => _hasData;
            set { SetProperty(ref _hasData, value); }
        }

        private DelegateCommand<ResignationEntity> _openEmployeeViewCommand = null;
        public DelegateCommand<ResignationEntity> OpenEmployeeViewCommand => _openEmployeeViewCommand ?? (_openEmployeeViewCommand = new DelegateCommand<ResignationEntity>(OpenEmployeeView));

        private DelegateCommand<ResignationEntity> _openTTLinkCommand = null;
        public DelegateCommand<ResignationEntity> OpenTTLinkCommand => _openTTLinkCommand ?? (_openTTLinkCommand = new DelegateCommand<ResignationEntity>(OpenTTLink));

        private DelegateCommand _exportResignationsCommand = null;
        public DelegateCommand ExportResignationsCommand => _exportResignationsCommand ?? (_exportResignationsCommand = new DelegateCommand(ExportData));

        public ResignationsPageViewModel(IEventAggregator eventAggregator)
        {
            _isCurrentPage = false;
            _eventAggregator = eventAggregator;
            _repository = new ResignationsRepository();
            ResignationsList = new ObservableCollection<ResignationEntity>();

        }

        private async void GetData()
        {
            if (!_isCurrentPage) return;

            ResignationsList.Clear();

            WidgedState = HomePageWidgetState.SummaryLoading;

            var data = await _repository.GetAll();
            ResignationsList.AddRange(data);
            HasData = ResignationsList.Count > 0;

            WidgedState = HomePageWidgetState.SummaryLoaded;
        }

        private async void ExportData()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv", "Save resignations data");
            var path = dialog.ShowSaveDialog();
            if (string.IsNullOrEmpty(path)) return;

            var csvStream = new CSVStream(path);
            var dataManager = new DataManager();
            await dataManager.WriteToCsvAsync(csvStream, ResignationsList);

            FileHelper.RunProcess(path);
        }


        private void OpenEmployeeView(ResignationEntity entity)
        {
            var empl = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(entity.EmployeeID)).FirstOrDefault();
            if (empl == null)
            {
                empl = new Roster(entity);
            }

            _eventAggregator.GetEvent<NavigationEmplArgs>().Publish(("EmployeeData", empl));
        }

        private void OpenTTLink(ResignationEntity entity)
        {
            FileHelper.RunProcess(entity.TTLink);
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
