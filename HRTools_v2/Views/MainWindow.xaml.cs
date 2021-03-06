using AutoUpdaterDotNET;
using Domain.Storage;
using HRTools_v2.Helpers;
using HRTools_v2.ViewModels;
using HRTools_v2.Views.Awal;
using HRTools_v2.Views.Dashboard;
using HRTools_v2.Views.Meetings;
using HRTools_v2.Views.Resignations;
using HRTools_v2.Views.Sanctions;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

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
        private AppSettings _appSettings;
        private AwalPage _awal;
        private ResignationsPage _resignations;
        private SanctionsPage _sanctions;
        private MeetingsPage _meetings;
        private CustomMeetingsPage _customMeetings;
        private DashboardPage _dashboard;

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

            if (!Environment.UserName.Equals("eslut"))
            {
                AutoUpdater.ShowSkipButton = false;
                AutoUpdater.RunUpdateAsAdmin = false;
                AutoUpdater.Start(DataStorage.AppSettings.UpdatePath);

                DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(10) };
                timer.Tick += delegate
                {
                    AutoUpdater.Start(DataStorage.AppSettings.UpdatePath);
                };
                timer.Start();

            }

            _loaderPage = _container.Resolve<LoaderPage>();
            _homePage = _container.Resolve<HomePage>();
            _dashboard = _container.Resolve<DashboardPage>();
            _employeeData = _container.Resolve<EmployeeData>();
            _appSettings = _container.Resolve<AppSettings>();
            _awal = _container.Resolve<AwalPage>();
            _resignations = _container.Resolve<ResignationsPage>();
            _sanctions = _container.Resolve<SanctionsPage>();
            _meetings = _container.Resolve<MeetingsPage>();
            _customMeetings = _container.Resolve<CustomMeetingsPage>();

            _region = _regionManager.Regions["ContentRegion"];
            _ = _region.Add(_loaderPage);
            _ = _region.Add(_homePage);
            _ = _region.Add(_dashboard);
            _ = _region.Add(_employeeData);
            _ = _region.Add(_appSettings);
            _ = _region.Add(_awal);
            _ = _region.Add(_resignations);
            _ = _region.Add(_sanctions);
            _ = _region.Add(_meetings);
            _ = _region.Add(_customMeetings);
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
