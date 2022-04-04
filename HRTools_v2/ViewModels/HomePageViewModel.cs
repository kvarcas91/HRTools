using Domain.Models;
using Domain.Repository;
using Domain.Types;
using Prism.Mvvm;
using Prism.Regions;

namespace HRTools_v2.ViewModels
{
    public class HomePageViewModel : BindableBase, INavigationAware
    {

        #region Properties

        private DataPreview _previewDataSnip;
        public DataPreview PreviewDataSnip
        {
            get => _previewDataSnip;
            set { SetProperty(ref _previewDataSnip, value); }
        }

        private HomePageWidgetState _widgedState;
        public HomePageWidgetState WidgedState
        {
            get => _widgedState;
            set { SetProperty(ref _widgedState, value); }
        }

        #endregion

        private bool _isPageActive;
        private readonly PreviewRepository _previewRepository;

        public HomePageViewModel()
        {
            _previewRepository = new PreviewRepository();
        }

        private async void GetPreviewData()
        {
            WidgedState = HomePageWidgetState.SummaryLoading;
            PreviewDataSnip = await _previewRepository.GetPreviewAsync();
            WidgedState = HomePageWidgetState.SummaryLoaded;
        }

        #region Navigation

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
            GetPreviewData();
        }

        #endregion

    }
}
