using System.Windows;
using HRTools_v2.Views;
using Prism.Ioc;
using Prism.Unity;

namespace HRTools_v2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
