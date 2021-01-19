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
        ///   何度も同じストーリーボードを生成するのは無駄なので、キャッシュをしよう
        /// </summary>
        private Dictionary<(string, double), Storyboard> _storyboardCache = new();

        private int _viewRatio = 1;
        /// <summary>
        ///   表示倍率
        /// </summary>
        public int ViewRatio {
            get => _viewRatio;
            set {
                _viewRatio = value;
                Width = BaseWidth * ViewRatio;
                Height = BaseHeight * ViewRatio;
            }
        }
        public int BaseWidth => 30;
        public int BaseHeight => 30;

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
            // 何故かここで値をセットしないと正しく動かない。ことはなかった
            //CanvasLeft = left;
            //CanvasTop = top;
        }

        /// <summary>
        ///   <para>四角形を動かす。移動量は value * 20</para>
        ///   <para>X,Y 2つとも動くことはないので、両方値が入っている場合Xを動かす</para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        private void Animation(double left = 0, double top = 0)
        {
            (string, double) animeInfo = (left, top) switch {
                (0, 0) => (null, double.NaN),
                (_, 0) => ("(Canvas.Left)", left),
                (0, _) => ("(Canvas.Top)", top),
                _ => (null, double.NaN)             // ここには絶対来ないけどね…
            };
            // 移動量なし
            if(animeInfo.Item1 is null) return;
            Storyboard storyboard;
            // このアニメーションのストーリーは初めて
            if(!_storyboardCache.TryGetValue(animeInfo, out storyboard)) {
                Debug.WriteLine("ストーリーボード作成");
                // ストーリーボード作成
                storyboard = new Storyboard();
                storyboard.Children.Add(CreateAnimation(animeInfo));
                // ストーリーボードをキャッシュに追加
                _storyboardCache.Add(animeInfo, storyboard);
            }

            // アニメーションを開始します
            Debug.WriteLine("アニメーション");
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
            anime.Duration = TimeSpan.FromMilliseconds(100);

            return anime;
        }
    }
}
