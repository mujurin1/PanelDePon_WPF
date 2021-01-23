using PanelDePon.Types;
using PanelDePon_WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    public partial class PlayAreaPanel : UserControl
    {
        //===================================ここから============================
        public static readonly DependencyProperty PanePonPlayAreaServiceProperty =
            DependencyProperty.Register(
                nameof(PlayAreaSerivce),
                typeof(IPanelDePonPlayAreaService),
                typeof(PlayAreaPanel),
                new FrameworkPropertyMetadata(
                    null,   // nameof(PlayAreaSerivce)
                    new PropertyChangedCallback(OnPlayAreaSerivceChanged)
                ));

        private IPanelDePonPlayAreaService _panePonPlayAreaSerivce;
        /// <summary>
        ///   表示するパネポンのプレイエリアのインターフェース
        /// </summary>
        public IPanelDePonPlayAreaService PlayAreaSerivce {
            get => (IPanelDePonPlayAreaService)GetValue(PanePonPlayAreaServiceProperty);
            set => SetValue(PanePonPlayAreaServiceProperty, value);
        }

        private static void OnPlayAreaSerivceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = obj as PlayAreaPanel;
            if(ctrl is not null)
                ctrl._panePonPlayAreaSerivce = ctrl.PlayAreaSerivce;
            ctrl.PlayAreaInit();
        }
        //=============================ここまでバインド用のやつ====================

        /// <summary>今は固定値だけど、ホントは違う</summary>
        private static readonly int CellSize = 30;
        /// <summary>カーソル</summary>
        private SwapCursor _cursor;


        public PlayAreaPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   PlayAreaSerivce が変更された時に、表示を初期化する
        /// </summary>
        private void PlayAreaInit()
        {
            // フレーム更新時のイベントを追加
            _panePonPlayAreaSerivce.Updated += (_, _) => Update();

            // 画面サイズ変更
            Width = 30 * PlayAreaSerivce.PlayAreaSize.Column;
            Height = 30 * PlayAreaSerivce.PlayAreaSize.Row;

            // 表示セルの初期化
            // TODO: お邪魔セルを表示させる（今はお邪魔はやってない
            PanePonCanvas.Children.Clear();
            for(int row = -1; row < PlayAreaSerivce.PlayAreaSize.Row; row++) {
                for(int col = 0; col < PlayAreaSerivce.PlayAreaSize.Column; col++) {
                    var cell = this[row, col];
                    // 空のセルは表示する必要が無いので 戻る
                    if(cell.CellType is CellType.Empty) continue;
                    // お邪魔セルは表示できないので 戻る
                    if(cell.CellType is CellType.Ojama or CellType.HardOjama) continue;
                    // セルコントロールを生成して、キャンバスに追加
                    PanePonCanvas.Children.Add(
                        new PazzleCell(col * CellSize, row * CellSize) {
                            Cell = cell.CellType
                        });
                }
            }

            // カーソルの初期化
            this._cursor = new SwapCursor(
                            PlayAreaSerivce.CursorStatus.CursorPos.Column * CellSize,
                            PlayAreaSerivce.CursorStatus.CursorPos.Row * CellSize);
            PanePonCanvas.Children.Add(_cursor);
        }

        /// <summary>
        ///   表示を更新する
        /// </summary>
        private void Update()
        {


            // カーソルの更新 ホントは最大でも片方しか動かないけど、これでいいや
            _cursor.CanvasLeft = PlayAreaSerivce.CursorStatus.CursorPos.Column * CellSize;
            _cursor.CanvasBottom = PlayAreaSerivce.CursorStatus.CursorPos.Row * CellSize;
        }

        /// <summary>
        ///   キャンバス内のセルを取得する
        /// </summary>
        public CellInfo this[int row, int col] {
            get => PlayAreaSerivce.CellArray[row, col];
        }
    }
}
