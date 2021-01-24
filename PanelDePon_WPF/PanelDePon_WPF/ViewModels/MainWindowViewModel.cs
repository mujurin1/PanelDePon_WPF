using System;
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
        public ReactiveCommand<string> KeyInputCommand { get; set; } = new();

        public MainWindowViewModel(IPanelDePonPlayAreaService panePonService)
        {
            this._panePonService = panePonService;
            this.KeyInputCommand.Subscribe(key => GameUpdate(key));
        }

        //public enum UserOperation
        //{
        //    NaN = 0,                // 入力なし
        //    CursorUp = 1,           // カーソル上移動
        //    CursorLeft = 2,         // カーソル左移動
        //    CursorRight = 3,        // カーソル右移動
        //    CursorDown = 4,         // カーソル下移動
        //    ClickChangeCell = 5,    // クリックで入れ替える（カーソル移動＋入れ替え）、まだ未実装
        //    ChangeCell = 6,         // セルを入れ替える
        //    ScrollSpeedUp = 7,      // スクロール速度を早くする
        //}
        private void GameUpdate(string key)
        {
            var input = key switch {
                "Up" => UserOperation.CursorUp,
                "Left" => UserOperation.CursorLeft,
                "Right" => UserOperation.CursorRight,
                "Down" => UserOperation.CursorDown,
                "Swap" => UserOperation.Swap,
                "SpeedUp" => UserOperation.ScrollSpeedUp,
                _ => UserOperation.NaN,
            };
            _panePonService.UpdateFrame(input);
        }
    }
}
