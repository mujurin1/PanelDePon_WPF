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
using Matrix = PanelDePon.Types.Matrix;

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    /* ・初期化の流れ
     * 1. PlayAreaSerivce に、 IPanelDePonPlayAreaService をセットする
     * 2. PlayAreaInit() が呼ばれる
     * 3. 存在するセル分の PazzleCellコントロール を作り、表示する
     *    （自分のコントロール内にある PanePonCanvas に作成した PazzleCell を追加する）
     * 
     * ・アップデートの流れ
     * 1. PlayAreaService がアップデートを実行する（外部）
     * 2. PlayAreaSerivce から、アップデート呼び出される
     * 3. スクロール位置の更新
     * 4. コントロール内のセルに、 PlayAreaService.CellArray の対応した値を渡してアップデートを呼ぶ
     * 
     * 
     */
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

        private IPanelDePonPlayAreaService _playAreaSerivce;
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
                ctrl._playAreaSerivce = ctrl.PlayAreaSerivce;
            ctrl.PlayAreaInit();
        }
        //=============================ここまでバインド用のやつ====================

        /// <summary>カーソル</summary>
        private SwapCursor _cursor;
        ///// <summary>PlayAreaCell内に表示しているセル</summary>
        //private List<PazzleCell> _pazzleCells = new();

        public PlayAreaPanel()
        {
            InitializeComponent();
            Canvas.SetBottom(CellCanvas, 0);
            Canvas.SetBottom(CursorCanvas, 0);
        }

        /// <summary>
        ///   PlayAreaSerivce が変更された時に、表示を初期化する
        /// </summary>
        private void PlayAreaInit()
        {
            // プレイエリア更新後イベントに追加
            _playAreaSerivce.Updated += (_, _) => Update();
            // 表示コントロールの削除
            CellCanvas.Children.Clear();
            CursorCanvas.Children.Clear();
            // 画面サイズ変更
            Width = 30 * PlayAreaSerivce.PlayAreaSize.Column;
            Height = 30 * PlayAreaSerivce.PlayAreaSize.Row;

            // カーソルの初期化
            this._cursor = new SwapCursor(_playAreaSerivce, PlayAreaSerivce.CursorStatus.Matrix);
            CursorCanvas.Children.Add(_cursor);
            // 表示セルの初期化
            // TODO: お邪魔セルを表示させる（今はお邪魔はやってない
            for(int row = -1; row < PlayAreaSerivce.PlayAreaSize.Row; row++)
                foreach(var cell in CreatePazzleCellColumn(row))
                    CellCanvas.Children.Add(cell);
        }

        /// <summary>
        ///   表示を更新する
        /// </summary>
        private void Update()
        {
            // ===============================PlayAreaCanvas の更新
            // カーソル
            _cursor.Update();
            // セル
            var removeCells = new List<PazzleCell>();
            foreach(var ctrl in CellCanvas.Children) {
                var cell = ctrl as PazzleCell;
                // セルの更新    お邪魔から普通のセルに変身するとき、そのお邪魔の範囲を返す
                if(cell.Update() is Matrix matrix) {
                    // TODO: 範囲を持ったセル（お邪魔）から、単体のセルになった時
                    // matrix: そのセルのいた範囲
                }
                // 削除するセル
                if(cell.IsRemove) removeCells.Add(cell);
            }
            // 消滅したセルを削除
            foreach(var cell in removeCells)
                CellCanvas.Children.Remove(cell);
            // ===============================スクロールの更新
            Canvas.SetBottom(CellCanvas, _playAreaSerivce.ScrollPer * 30);
            Canvas.SetBottom(CursorCanvas, _playAreaSerivce.ScrollPer * 30);
            if(_playAreaSerivce.ScrollLine == 0) {      // ちょうどせり上がった
                //// キャンバス内のコントロールを１段上に上げる
                //// カーソル
                //_cursor.Move(new(_cursor.Matrix.Row+1, _cursor.Matrix.Column), isAnimation:false);
                //// セル
                //foreach(var ctrl in CellCanvas.Children) {
                //    var cell = ctrl as PlayAreaControlAbs;
                //    cell.Move(new(cell.Matrix.Row+1, cell.Matrix.Column), isAnimation:false);
                //}
                // 一番下に、新しくセルを生成する
                foreach(var cell in CreatePazzleCellColumn(row: -1))
                    CellCanvas.Children.Add(cell);
            }
        }

        /// <summary>
        ///   横一列分の、セルを作って返す
        /// </summary>
        /// <param name="row">生成する段</param>
        /// <returns>生成したパズルセル１段</returns>
        private List<PazzleCell> CreatePazzleCellColumn(int row)
        {
            var cells = new List<PazzleCell>();
            for(int col = 0; col < PlayAreaSerivce.PlayAreaSize.Column; col++) {
                var cell = this[row, col];
                // 空のセルは表示する必要が無いので 戻る
                if(cell.CellType is CellType.Empty) continue;
                // お邪魔セルは表示できないので 戻る
                if(cell.CellType is CellType.Ojama or CellType.HardOjama) continue;
                // セルコントロールを生成して、キャンバスに追加
                var matrix = new Matrix(row, col);
                var c = new PazzleCell(_playAreaSerivce, matrix);
                //c.Opacity = 0.1;  // 透明度 透明0 ~ 1不透明
                cells.Add(c);
            }
            return cells;
        }

        /// <summary>
        ///   キャンバス内のセルを取得する
        /// </summary>
        public CellInfo this[int row, int col] {
            get => PlayAreaSerivce.CellArray[row, col];
        }
    }
}
