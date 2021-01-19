using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    /// <summary>
    ///   <para>パネポンコントローラーの共通基底抽象クラス</para>
    ///   <para>全てのコントローラーに共通する機能を書く</para>
    /// </summary>
    public abstract class PanePonControlAbs : UserControl
    {
        private static double _scale = 1;
        /// <summary>表示倍率</summary>
        public static double Scale {
            get => _scale;
            set {
                var old = _scale;   // 古い値を保持
                _scale = value;
                foreach(var weakRefe in _createControls) {
                    PanePonControlAbs control;
                    if(weakRefe.TryGetTarget(out control))
                        control.ChangeScale(old);
                }
            }
        }

        public virtual int BaseWidth { get; }
        public virtual int BaseHeight { get; }

        /// <summary>
        ///   <para>Scaleが変更されたら継承した全てのクラスのインスタンスに通知を飛ばしたい</para>
        ///   <para>そのために、生成した全てのインスタンスを"弱参照"で保持しておく</para>
        ///   <para>https://ufcpp.net/study/csharp/RmWeakReference.html</para>
        /// </summary>
        private static List<WeakReference<PanePonControlAbs>> _createControls = new();
        /// <summary>自分自身への弱参照</summary>
        private WeakReference<PanePonControlAbs> weakReference;

        public PanePonControlAbs()
        {
            // 下の値が設定されてないと例外が出るので初期値指定（しかも気が付きにくいエラー…）
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);

            this.weakReference = new(this);
            _createControls.Add(weakReference);
            ChangeScale(1);
        }

        /// <summary>
        ///   スケールが変更されたときに呼ばれる
        /// </summary>
        /// <param name="oldScale">変更前のスケール</param>
        protected abstract void ChangeScale(double oldScale);
    }
}
