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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Matrix = PanelDePon.Types.Matrix;

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    /// <summary>
    ///   <para>パネポンの動かして消すセル</para>
    ///   <para>Canvas.Left 等は使用禁止</para>
    /// </summary>
    public partial class PazzleCell : PlayAreaControlAbs
    {
        /// <summary>表示するセルの情報</summary>
        public CellInfo CellInfo { get; private set; }
        /// <summary>このセルはもう表示しない。消してくれ</summary>
        public bool IsRemove { get; private set; } = false;
        /// <summary>セルの移動方向  -1:静止 0:上 1:左 2:右 3:下</summary>
        private (int dir, int time) _direction = (-1, 0);

        public PazzleCell(IPanelDePonPlayAreaService playAreaService, Matrix matrix) : base(playAreaService, matrix)
        {
            InitializeComponent();
            this.BorderBrush = Brushes.Black;
            this.BorderThickness = new Thickness(1);
            this.CellInfo = _playAreaService.CellArray[matrix];
            CellInit();
        }

        /// <summary>
        ///   表示の初期化
        /// </summary>
        private void CellInit()
        {
            Width = CellSize;
            Height = CellSize;
            this.Background = CellInfo.CellType switch {
                CellType.Red => Brushes.Red,
                CellType.Blue => Brushes.Blue,
                CellType.Sky => Brushes.SkyBlue,
                CellType.Yellow => Brushes.Yellow,
                CellType.Green => Brushes.Green,
                CellType.Bikkuri => Brushes.Silver,
                _ => throw new Exception($"非対応セル Type: {CellInfo.CellType}")
            };
        }

        /// <summary>
        ///   表示の更新。自分が表示するセルの情報で更新する
        /// </summary>
        /// <remarks>お邪魔から、お邪魔顔セルになったときに、生成するセルのエリアを返す</remarks>
        public Matrix? Update()
        {
            // スクロール
            if(_playAreaService.ScrollPer == 0) {
                var m = Matrix;
                m.Row++;
                Matrix = m;
            }

            // もし対応したセルが移動していたら移動方向を調べる
            if(_playAreaService.SwapArray[Matrix] is Matrix matrix) {
                // 古い情報を保管して、移動させる
                var old = Matrix;
                this.Matrix = matrix;
                // セルの情報を更新
                CellInfo = _playAreaService.CellArray[matrix];
                // 移動量と移動時間を設定
                _direction.time = CellInfo.StateTimer.Lock;
                _direction.dir = (Matrix.Row - old.Row, Matrix.Column - old.Column) switch {
                    (0, -1) => 1,   // 左方向
                    (0, 1) => 2,    // 右方向
                    (-1, 0) => 3,   // 下方向
                    _ => throw new Exception($"変な方向への移動 new:{(Matrix)}  old:{old}"),
                };
            } else  // 移動して無くても情報を更新
                CellInfo = _playAreaService.CellArray[Matrix];

            // セルの位置の設定、初期値
            double bottom = Matrix.Row * CellSize;
            double left = Matrix.Column * CellSize;
            // セルが移動していたら移動位置を変える
            if(_direction.time is > 0) {
                // セルの現在の移動割合
                double directionPer = CellSize * CellInfo.StateTimer.Lock / CellInfo.TransitionTime;
                (bottom, left) = _direction.dir switch {
                    1 => (bottom, left + directionPer),     // 左方向
                    2 => (bottom, left - directionPer),     // 右方向
                    3 => (bottom + directionPer, left),     // 下方向
                    _ => throw new Exception("警告対策"),
                };
                _direction.time--;
            }
            // セルを移動させる
            Canvas.SetBottom(this, bottom);
            Canvas.SetLeft(this, left);

            /* ・更新内容
             * * セルの移動
             * * セルの点滅・顔・消滅変身
             */
            switch(CellInfo.Status) {
            case (CellState.Free):      // 自分の表示を削除 or 何もしない
                if(CellInfo.CellType is CellType.Empty) {
                } else {
                    // 何も起きてない状態
                    Moji.Text = "普通";
                }
                break;
            case (CellState.Lock):      // 移動 or 消滅変身後待機
                if(CellInfo.CellType is CellType.Empty) {           // 種類：空     普通のセルが消滅した
                    // 自分の削除フラグを立てる
                    IsRemove = true;
                    //Moji.Text = $"消{CellInfo.StateTimer.Lock}";
                } else if(CellInfo.CellType.IsNomal()) {            // 種類：普通   お邪魔が普通のセルになった

                    Moji.Text = $"動{CellInfo.StateTimer.Lock}";
                    // TODO: 見た目を普通のセルに
                } else {
                    throw new Exception("セルの状態がおかしい");
                }
                break;
            case (CellState.Flash):     // 点滅
                // TODO: フラッシュ
                Moji.Text = $"点{CellInfo.StateTimer.Flash}";
                break;
            case (CellState.Neutral):   // 顔
                Moji.Text = $"顔{CellInfo.StateTimer.Neutral}";
                // TODO: 顔

                // 範囲を持ったお邪魔から、１マスの顔の表示のお邪魔に変わり、
                // 自分のいた範囲を返す
                return new Matrix(0, 0);
            case (CellState.Moment):    // 消滅変身
                Moji.Text = $"変{CellInfo.StateTimer.Neutral}";
                // TODO: 消滅変身
                break;
            }
            return null;
        }
    }
}
