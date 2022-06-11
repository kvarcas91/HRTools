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
using System.Collections.ObjectModel;
using System.Linq;

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

        private bool _isCustomViewOn;
        public bool IsCustomViewOn
        {
            get => _isCustomViewOn;
            set { SetProperty(ref _isCustomViewOn, value); }
        }

        private string _selectedCreator;
        public string SelectedCreator
        {
            get => _selectedCreator;
            set { SetProperty(ref _selectedCreator, value); GetData(); }
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

        private ObservableCollection<string> _sanctionsCreatorList;
        public ObservableCollection<string> SanctionsCreatorList
        {
            get { return _sanctionsCreatorList; }
            set { SetProperty(ref _sanctionsCreatorList, value); }
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

        private DelegateCommand _importSanctionsCommand = null;
        public DelegateCommand ImportSanctionsCommand => _importSanctionsCommand ?? (_importSanctionsCommand = new DelegateCommand(ImportSanctions));

        private DelegateCommand _getAllCommand = null;
        public DelegateCommand GetAllCommand => _getAllCommand ?? (_getAllCommand = new DelegateCommand(GetData));


        public SanctionsPageViewModel(IEventAggregator eventAggregator)
        {
            ShowAllSanctions = false;
            HasData = true;
            WidgedState = HomePageWidgetState.SummaryLoading;
            _eventAggregator = eventAggregator;
            _repository = new SanctionsRepository();
            SanctionsList = new ObservableCollection<SanctionEntry>();
            SanctionsCreatorList = new ObservableCollection<string>();
            _selectedCreator = "All";
        }

        private async void SetSanctionCreators()
        {
            SanctionsCreatorList.Clear();
            SanctionsCreatorList.Add("All");
            SanctionsCreatorList.AddRange(await _repository.GetAllSanctionCreatorsAsync());
        }

        private async void GetData()
        {
            if (!_isCurrentPage) return;

            IsCustomViewOn = false;
            SanctionsList.Clear();

            WidgedState = HomePageWidgetState.SummaryLoading;

            SanctionsList.AddRange(await _repository.GetAllAsync(ShowAllSanctions, SelectedCreator));
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

        private async void ImportSanctions()
        {
            var dialog = new DialogHelper(".csv", "CSV Files|*.csv");
            var path = dialog.ShowOpenDialog();
            if (string.IsNullOrEmpty(path)) return;

            SanctionsList.Clear();
            WidgedState = HomePageWidgetState.SummaryLoading;
            HasData = true;

            var objList = await _repository.GetAllAsync(false, "All");

            if (objList == null || objList.ToList().Count == 0)
            {
                SendToast("There are no live sanctions", NotificationType.Information);
                GetData();
                return;
            }

            var csvReader = new CSVStream(path);
            var idList = csvReader.GetSingleColumn(0);

            if (idList == null || idList.Count == 0)
            {
                GetData();
                SendToast("Failed to load ID list", NotificationType.Error);
                return;
            }

            foreach (var item in idList)
            {
                SanctionEntry sanction = objList.Where(x => x.EmployeeID.Equals(item)).FirstOrDefault();
                if (!string.IsNullOrEmpty(sanction.ID))
                {
                    SanctionsList.Add(sanction);
                }
                
            }

            HasData = SanctionsList.Count > 0;
            IsCustomViewOn = true;
            WidgedState = HomePageWidgetState.SummaryLoaded;


        }

        private void SendToast(string message, NotificationType notificationType)
        {
            _eventAggregator.GetEvent<ShowToastArgs>().Publish((message, notificationType));
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
            SetSanctionCreators();
            GetData();
        }

        #endregion
    }
}
