﻿using PanelDePon.Types;
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
        /// <summary>セルの移動方向  -1:静止 0:上 1:左 2:右 3:下</summary>
        private (int dir, int time) _direction = (-1, 0);

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
            // スクロール
            if(_playAreaService.ScrollPer == 0) {
                var m = Matrix;
                m.Row++;
                Matrix = m;
            }

            // もし対応したセルが移動していたら移動方向を調べる
            if(_playAreaService.SwapArray[Matrix] is Matrix matrix) {
                // 自分が表示するセルの情報を取得
                CellInfo = _playAreaService.CellArray[matrix];

                var old = Matrix;
                switch((matrix.Row-Matrix.Row, matrix.Column - Matrix.Column)) {
                case (0, -1):   // 左方向
                    _direction = (1, CellInfo.StateTimer.Lock); break;
                case (0, 1):    // 右方向
                    _direction = (2, CellInfo.StateTimer.Lock); break;
                case (-1, 0):   // 下方向
                    _direction = (3, CellInfo.StateTimer.Lock); break;
                default:        // 上方向
                    throw new Exception($"上方向への移動はありえません  new:{(matrix)}  old:{Matrix}");
                }
                this.Matrix = matrix;
            } else  // 移動して無くても情報を更新
                CellInfo = _playAreaService.CellArray[Matrix];

            // ちょっと、やだけど、毎回位置を指定し直そう
            if(_direction.time is 0) {
                Canvas.SetLeft(this, Matrix.Column * CellSize);
                Canvas.SetBottom(this, Matrix.Row * CellSize);
            } else {
                switch(_direction.dir) {
                case (1):   // 左
                    Canvas.SetLeft(this,
                        Matrix.Column * CellSize + CellSize * (CellInfo.StateTimer.Lock / CellInfo.TransitionTime));
                    break;
                case (2):   // 右
                    Canvas.SetLeft(this,
                        Matrix.Column * CellSize - CellSize * (CellInfo.StateTimer.Lock / CellInfo.TransitionTime));
                    break;
                case (3):   // 下
                    Canvas.SetBottom(this,
                        Matrix.Row * CellSize + CellSize * (CellInfo.StateTimer.Lock / CellInfo.TransitionTime));
                    break;
                }
                _direction.time--;
            }

            /* ・更新内容
             * * セルの移動
             * * セルの点滅・顔・消滅変身
             */
            // アップデート内容：セルの点滅・顔・消滅
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
