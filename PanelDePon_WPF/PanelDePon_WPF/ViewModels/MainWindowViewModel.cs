using PanelDePon.Types;
using PanelDePon_WPF.Services.Interfaces;
using Prism.Mvvm;
using Reactive.Bindings;
using System.Diagnostics;

namespace PanelDePon_WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IPanelDePonPlayAreaService _panePonService;

        private string _title = "Prism Application";
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public ReactiveCommand UpCommand { get; set; } = new ReactiveCommand();

        public MainWindowViewModel(IPanelDePonPlayAreaService panePonService)
        {
            this._panePonService = panePonService;
            this.UpCommand.Subscribe(KeyUp);
        }
        bool a = true;
        private void KeyUp()
        {
            var input = UserOperation.NaN;
            if(a) input = UserOperation.ChangeCell;
            _panePonService.UpdateFrame(input);
            a = false;
        }
    }
}
