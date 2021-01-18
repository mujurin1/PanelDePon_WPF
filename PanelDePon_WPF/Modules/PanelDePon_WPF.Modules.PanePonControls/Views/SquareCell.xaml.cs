using PanelDePon_WPF.Modules.PanePonControls.Converters;
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

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    /// <summary>
    ///   <para>スムーズに動く四角形</para>
    ///   <para>Canvas.xxは使用禁止</para>
    /// </summary>
    public partial class SquareCell : UserControl
    {
        /// <summary>
        ///   何度も同じアニメーションを生成するのは無駄なので、キャッシュしておく
        /// </summary>
        private Dictionary<(string, double), DoubleAnimation> _animeCache = new();

        public double CanvasLeft {
            get => Canvas.GetLeft(this);
            set {
                Animation(left: value - Canvas.GetLeft(this));
                //SetValue(Canvas.LeftProperty, value);
                //Canvas.SetLeft(this, value);
            }
        }
        public double CanvasTop {
            get => Canvas.GetTop(this);
            set {
                Animation(top: value - Canvas.GetTop(this));
                //SetValue(Canvas.TopProperty, value);
                //Canvas.SetTop(this, value);
            }
        }

        public SquareCell(double left = 0, double top = 0)
        {
            InitializeComponent();

            // 下の値が設定されてないと例外が出るので初期値指定（しかも気が付きにくいエラー…）
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
            // 何故かここで値をセットしないと正しく動かない
            CanvasLeft = left;
            CanvasTop = top;
        }

        /// <summary>
        ///   <para>四角形を動かす。移動量は value * 20</para>
        ///   <para>X,Y 2つとも動くことはないので、両方値が入っている場合Xを動かす</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        private void Animation(double left = 0, double top = 0)
        {
            var anime = (left, top) switch {
                (0, 0) => null,
                (_, 0) => CreateAnimation("(Canvas.Left)", left),
                (0, _) => CreateAnimation("(Canvas.Top)", top),
                _ => null
            };
            if(anime is null) return;
            var storyboard = new Storyboard();
            storyboard.Children.Add(anime);
            // アニメーションを開始します
            storyboard.Begin();
        }

        /// <summary>
        ///   DoubleAnimation を生成して返す簡易メソッド
        /// </summary>
        /// <param name="propertyPath">アニメーションさせるプロパティ名</param>
        /// <param name="value">移動量</param>
        /// <returns></returns>
        private DoubleAnimation CreateAnimation(string propertyPath, double value)
        {
            DoubleAnimation anime;
            if(_animeCache.TryGetValue((propertyPath, value), out anime)) {
                Debug.WriteLine("きゃっしゅから");
                return anime;
            }
            anime = new DoubleAnimation();
            // TargetName添付プロパティではなく、Target添付プロパティで
            // 直接アニメーションのターゲットを指定しています。
            Storyboard.SetTarget(anime, this);
            Storyboard.SetTargetProperty(anime, new PropertyPath(propertyPath));
            anime.By = value;
            anime.Duration = TimeSpan.FromMilliseconds(100);

            _animeCache.Add((propertyPath, value), anime);
            return anime;
        }
    }
}
