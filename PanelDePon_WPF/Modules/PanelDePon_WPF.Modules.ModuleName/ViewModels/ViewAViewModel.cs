using PanelDePon_WPF.Core.Mvvm;
using PanelDePon_WPF.Services.Interfaces;
using Prism.Regions;
using System.Windows.Media.Animation;

namespace PanelDePon_WPF.Modules.ModuleName.ViewModels
{
    public class ViewAViewModel : RegionViewModelBase
    {

        public ViewAViewModel(IRegionManager regionManager, IMessageService messageService) :
            base(regionManager)
        {
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            //do something
        }
    }
}
