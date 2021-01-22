using PanelDePon_WPF.Core.Mvvm;
using PanelDePon_WPF.Services.Interfaces;
using Prism.Regions;
using Reactive.Bindings;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace PanelDePon_WPF.Modules.ModuleName.ViewModels
{
    public class ViewAViewModel : RegionViewModelBase
    {
        public ReactivePropertySlim<IPanelDePonPlayAreaService> PanePonService { get; set; }
            = new ReactivePropertySlim<IPanelDePonPlayAreaService>();

        public ViewAViewModel(IRegionManager regionManager,
                              IPanelDePonPlayAreaService panePonService) : base(regionManager)
        {
            this.PanePonService.Value = panePonService;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
