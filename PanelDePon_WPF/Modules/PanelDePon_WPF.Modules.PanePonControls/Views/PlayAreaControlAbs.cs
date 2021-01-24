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
using System.Windows.Media.Animation;
using Matrix = PanelDePon.Types.Matrix;

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    /// <summary>
    ///   <para>プレイエリアに表示するコントロールの共通抽象クラス</para>
    /// </summary>
    public abstract class PlayAreaControlAbs : UserControl
    {
        /// <summary>
        ///   移動アニメーション用のキャッシュ
        /// </summary>
        private readonly Dictionary<(string, double), Storyboard> _moveSBCache = new();

        protected readonly IPanelDePonPlayAreaService _playAreaService;
        /// <summary>アニメーション速度</summary>
        public virtual TimeSpan AnimeSpeed { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>セルのサイズ</summary>
        public int CellSize = 30;
        protected Matrix _matrix;
        /// <summary>
        ///   自分の存在位置。Matrix上
        /// </summary>
        public virtual Matrix Matrix {
            get => _matrix;
            set {
                MoveAnimation(left: (value.Column - _matrix.Column) * CellSize,
                              bottom: (value.Row - _matrix.Row) * CellSize);
                _matrix = value;
            }
        }

        //public double CanvasLeft {
        //    get => Canvas.GetLeft(this);
        //    set {
        //        Animation(left: value - Canvas.GetLeft(this));
        //    }
        //}
        //public double CanvasBottom {
        //    get => Canvas.GetBottom(this);
        //    set {
        //        Animation(bottom: value - Canvas.GetBottom(this));
        //    }
        //}

        /// <summary>
        ///   必ず、PlayAreaControlAbs(Matrix) の方を呼べ
        /// </summary>
        private PlayAreaControlAbs() { }

        public PlayAreaControlAbs(IPanelDePonPlayAreaService playAreaService, Matrix matrix)
        {
            this._playAreaService = playAreaService;
            this._matrix = matrix;
            Canvas.SetBottom(this, Matrix.Row * CellSize);
            Canvas.SetLeft(this, Matrix.Column * CellSize);
        }

        /// <summary>
        ///   <para>自分の位置をCanvas座標で動かす</para>
        ///   <para>Left,Bottom が同時に動くことはないので、両方値が入っている場合Leftを動かす</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        protected void MoveAnimation(double left = 0, double bottom = 0)
        {
            (string, double) animeInfo = (left, bottom) switch {
                (0, 0) => (null, double.NaN),       // 移動量なし
                (_, 0) => ("(Canvas.Left)", left),
                (0, _) => ("(Canvas.Bottom)", bottom),
                _ => throw new Exception($"PazzleCell Animation: Left, Bottom の値が不正です\nLeft {left}  Bottom {bottom}")  // NaNのまま進むとアニメーションで例外になるので
            };

            // 移動量なし
            if(animeInfo.Item1 is null) return;
            // このアニメーションのストーリーは初めて
            if(!_moveSBCache.TryGetValue(animeInfo, out Storyboard storyboard)) {
                // ストーリーボード作成
                storyboard = new Storyboard();
                storyboard.Children.Add(
                    CreateAnimation(this, animeInfo.Item1, animeInfo.Item2, AnimeSpeed));
                // ストーリーボードをキャッシュに追加
                _moveSBCache.Add(animeInfo, storyboard);
            }

            // アニメーションを開始します
            storyboard.Begin();
        }

        /// <summary>
        ///   DoubleAnimation を生成して返す簡易メソッド
        /// </summary>
        /// <param name="target">アニメーションする対象</param>
        /// <param name="propertyPath">アニメーションさせるプロパティ名</param>
        /// <param name="value">移動量</param>
        /// <param name="time">アニメーション時間</param>
        /// <returns></returns>
        protected static DoubleAnimation CreateAnimation(DependencyObject target, string propertyPath, double value, TimeSpan time)
        {
            var anime = new DoubleAnimation();
            // TargetName添付プロパティではなく、Target添付プロパティで
            // 直接アニメーションのターゲットを指定しています。
            Storyboard.SetTarget(anime, target);
            Storyboard.SetTargetProperty(anime, new PropertyPath(propertyPath));
            anime.By = value;
            anime.Duration = time;

            return anime;
        }
    }
}
