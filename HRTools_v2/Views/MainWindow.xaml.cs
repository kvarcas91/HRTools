using HRTools_v2.Helpers;
using HRTools_v2.ViewModels;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Interop;

namespace HRTools_v2.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;
        private IRegion _region;

        private LoaderPage _loaderPage;
        private HomePage _homePage;
        private EmployeeData _employeeData;

        public MainWindow(IContainerExtension container, IRegionManager regionManager)
        {
            SetWindow();
            InitializeComponent();
            InitializeResizer();
            SetWindowButtonActions();

            RegionManager.SetRegionManager(this, regionManager);

            _container = container;
            _regionManager = regionManager;

            Loaded += MainWindow_Loaded;
        }

        private void SetWindow()
        {
            MaxHeight = SystemParameters.VirtualScreenHeight;
            MaxWidth = SystemParameters.VirtualScreenWidth;
            MinHeight = 600;
            MinWidth = 800;
        }

        private void InitializeResizer()
        {
            SourceInitialized += (s, e) =>
            {
                IntPtr handle = new WindowInteropHelper(this).Handle;
                HwndSource.FromHwnd(handle).AddHook(new HwndSourceHook(WindowSizeHelper.WindowProc));
            };
        }

        private void SetWindowButtonActions()
        {
            CloseButton.Click += (s, e) => Close();
            MinimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;
            RestoreButton.Click += (s, e) =>
            {
                WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
                ResizeMode = WindowState == WindowState.Normal ? ResizeMode.CanResizeWithGrip : ResizeMode.NoResize;
            };
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            _loaderPage = _container.Resolve<LoaderPage>();
            _homePage = _container.Resolve<HomePage>();
            _employeeData = _container.Resolve<EmployeeData>();

            _region = _regionManager.Regions["ContentRegion"];
            _ = _region.Add(_loaderPage);
            _ = _region.Add(_homePage);
            _ = _region.Add(_employeeData);
        }

        private void OnSearchBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Down)
            {
                var dt = DataContext as MainWindowViewModel;
                dt.HandleSearch();
            }
        }
    }
}
