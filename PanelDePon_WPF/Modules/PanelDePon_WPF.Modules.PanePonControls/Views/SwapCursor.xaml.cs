using System;
using System.Collections.Generic;
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

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    /// <summary>
    /// SwapCursor.xaml の相互作用ロジック
    /// </summary>
    public partial class SwapCursor : UserControl
    {
        /// <summary>
        ///   何度も同じストーリーボードを生成するのは無駄なので、キャッシュをしよう
        /// </summary>
        private readonly Dictionary<(string, double), Storyboard> _storyboardCache = new();
        /// <summary>アニメーション速度</summary>
        private readonly static TimeSpan AnimeSpeed = TimeSpan.FromMilliseconds(100);

        public double CanvasLeft {
            get => Canvas.GetLeft(this);
            set {
                Animation(left: value - Canvas.GetLeft(this));
            }
        }
        public double CanvasBottom {
            get => Canvas.GetBottom(this);
            set {
                Animation(bottom: value - Canvas.GetBottom(this));
            }
        }

        public SwapCursor(double left = 0, double bottom = 0)
        {
            InitializeComponent();
            Canvas.SetLeft(this, left);
            Canvas.SetBottom(this, bottom);
            CanvasLeft = 0;
            CanvasBottom = 0;
            this.BorderBrush = Brushes.Pink;
            this.BorderThickness = new Thickness(1);

            Width = 60;
            Height = 30;
        }

        /// <summary>
        ///   <para>自分の位置をCanvas座標で動かす</para>
        ///   <para>Left,Bottom が同時に動くことはないので、両方値が入っている場合Leftを動かす</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        private void Animation(double left = 0, double bottom = 0)
        {
            (string, double) animeInfo = (left, bottom) switch {
                (0, 0) => (null, double.NaN),       // 移動量なし
                (_, 0) => ("(Canvas.Left)", left),
                (0, _) => ("(Canvas.Bottom)", bottom),
                _ => throw new Exception($"PazzleCell Animation: Left, Bottom の値が不正です\nLeft {left}  Bottom {bottom}")  // NaNのまま進むとアニメーションで例外になるので
            };

            if(animeInfo.Item1 is null) return;
            // このアニメーションのストーリーは初めて
            if(!_storyboardCache.TryGetValue(animeInfo, out Storyboard storyboard)) {
                // ストーリーボード作成
                storyboard = new Storyboard();
                storyboard.Children.Add(CreateAnimation(animeInfo));
                // ストーリーボードをキャッシュに追加
                _storyboardCache.Add(animeInfo, storyboard);
            }

            // アニメーションを開始します
            storyboard.Begin();
        }

        private DoubleAnimation CreateAnimation((string, double) animeInfo)
            => CreateAnimation(animeInfo.Item1, animeInfo.Item2);
        /// <summary>
        ///   DoubleAnimation を生成して返す簡易メソッド
        /// </summary>
        /// <param name="propertyPath">アニメーションさせるプロパティ名</param>
        /// <param name="value">移動量</param>
        /// <returns></returns>
        private DoubleAnimation CreateAnimation(string propertyPath, double value)
        {
            var anime = new DoubleAnimation();
            // TargetName添付プロパティではなく、Target添付プロパティで
            // 直接アニメーションのターゲットを指定しています。
            Storyboard.SetTarget(anime, this);
            Storyboard.SetTargetProperty(anime, new PropertyPath(propertyPath));
            anime.By = value;
            anime.Duration = AnimeSpeed;

            return anime;
        }
    }
}
