using Domain.Extensions;
using Domain.Models;
using Domain.Models.AWAL;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using HRTools_v2.Args;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HRTools_v2.ViewModels.Awal
{
    public class AwalPageViewModel : BindableBase, INavigationAware
    {

        private bool _isCurrentPage;

        private readonly AWALRepository _repository;
        private readonly IEventAggregator _eventAggregator;

        private ObservableCollection<AwalEntity> _awalList;
        public ObservableCollection<AwalEntity> AwalList
        {
            get { return _awalList; }
            set { SetProperty(ref _awalList, value); }
        }

        private HomePageWidgetState _widgedState;
        public HomePageWidgetState WidgedState
        {
            get => _widgedState;
            set { SetProperty(ref _widgedState, value); }
        }

        private bool _hasAwalData;
        public bool HasAwalData
        {
            get => _hasAwalData;
            set { SetProperty(ref _hasAwalData, value); }
        }

        private DelegateCommand<AwalEntity> _openEmployeeViewCommand = null;
        public DelegateCommand<AwalEntity> OpenEmployeeViewCommand => _openEmployeeViewCommand ?? (_openEmployeeViewCommand = new DelegateCommand<AwalEntity>(OpenEmployeeView));

        public AwalPageViewModel(AWALRepository repository, IEventAggregator eventAggregator)
        {
            _isCurrentPage = false;

            _awalList = new ObservableCollection<AwalEntity>();
            _repository = repository;
            _eventAggregator = eventAggregator;
        }

        #region Data Getters

        private async Task GetAwal()
        {
            if (!_isCurrentPage) return;

            AwalList.Clear();

            WidgedState &= ~HomePageWidgetState.EmployeeAwalSummaryLoaded;
            WidgedState |= HomePageWidgetState.EmployeeAwalSummaryLoading;


            var data = await _repository.GetAwalList();

            AwalList.AddRange(data);

            HasAwalData = AwalList.Count > 0;

            WidgedState &= ~HomePageWidgetState.EmployeeAwalSummaryLoading;
            WidgedState |= HomePageWidgetState.EmployeeAwalSummaryLoaded;
        }

        #endregion

        private void OpenEmployeeView(AwalEntity awal)
        {
            var empl = DataStorage.RosterList.Where(x => x.EmployeeID.Equals(awal.EmployeeID)).FirstOrDefault();
            if (empl == null)
            {
                empl = new Roster(awal);
            }

            _eventAggregator.GetEvent<NavigationEmplArgs>().Publish(("EmployeeData", empl));
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
            GetAwal();
        }

        #endregion

    }
}
