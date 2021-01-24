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
        private CellInfo _cell;
        /// <summary>表示するセルの情報</summary>
        public CellInfo CellInfo {
            get => _cell;
            private set {
                _cell = value;
                CellInit();
            }
        }
        /// <summary>このセルはもう表示しない。消してくれ</summary>
        public bool IsRemove { get; private set; } = false;

        public PazzleCell(IPanelDePonPlayAreaService playAreaService, Matrix matrix) : base(playAreaService, matrix)
        {
            InitializeComponent();
            this.BorderBrush = Brushes.Black;
            this.BorderThickness = new Thickness(1);
            this.CellInfo = _playAreaService.CellArray[matrix];
        }

        /// <summary>
        ///   表示の初期化
        /// </summary>
        private void CellInit()
        {
            Width = CellSize;
            Height = CellSize;
            switch(CellInfo.CellType) {
            case CellType.Red:
                this.Background = Brushes.Red;
                break;
            case CellType.Blue:
                this.Background = Brushes.Blue;
                break;
            case CellType.Sky:
                this.Background = Brushes.SkyBlue;
                break;
            case CellType.Yellow:
                this.Background = Brushes.Yellow;
                break;
            case CellType.Green:
                this.Background = Brushes.Green;
                break;
            case CellType.Bikkuri:
                this.Background = Brushes.Silver;
                break;
            }
        }

        /// <summary>
        ///   表示の更新。自分が表示するセルの情報で更新する
        /// </summary>
        /// <param name="cellInfo">表示するセルの情報</param>
        /// <remarks>お邪魔から、お邪魔顔セルになったときに、生成するセルのエリアを返す</remarks>
        public Matrix? Update()
        {
            // もし対応したセルが移動していた場合ずれているので、調べる
            // 移動していたら
            if(_playAreaService.SwapArray[Matrix] is Matrix matrix) {
                Matrix = matrix;
                CellInfo = _playAreaService.CellArray[Matrix];
                Moji.Text = $"動{CellInfo.StateTimer.Lock}";
                return null;
            }
            // 移動でなくても、自分が見るセルの情報を更新する
            CellInfo = _playAreaService.CellArray[Matrix];


            /* ・更新内容
             * * セルの移動
             * * セルの点滅・顔・消滅変身
             */
            // アップデート内容：セルの点滅・顔・消滅
            switch(CellInfo.Status) {
            case (CellState.Free):      // 何もない or 自分の表示を削除
                if(CellInfo.CellType is CellType.Empty) {
                    // 自分の削除フラグを立てる
                    IsRemove = true;
                } else {
                    // 何も起きてない状態
                    Moji.Text = "普通";
                }
                break;
            case (CellState.Lock):      // 移動 or 消滅変身後待機
                if(CellInfo.CellType is CellType.Empty) {           // 種類：空     普通のセルが消滅した
                    Moji.Text = $"消{CellInfo.StateTimer.Lock}";
                } else if(CellInfo.CellType.IsNomal()) {            // 種類：普通   お邪魔が普通のセルになった or 落下中
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
