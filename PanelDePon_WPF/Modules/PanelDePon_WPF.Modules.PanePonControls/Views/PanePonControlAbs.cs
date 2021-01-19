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
        /// <summary>
        ///   <para>Scaleが変更されたら継承した全てのクラスのインスタンスに通知を飛ばしたい</para>
        ///   <para>そのために、生成した全てのインスタンスを"弱参照"で保持しておく</para>
        ///   <para>https://ufcpp.net/study/csharp/RmWeakReference.html</para>
        /// </summary>
        private static readonly List<WeakReference<PanePonControlAbs>> _createControls = new();
        /// <summary>自分自身への弱参照</summary>
        private WeakReference<PanePonControlAbs> weakReference;

        /// <summary>パズルセルのサイズ</summary>
        public readonly static int CellSize = 30;

        public PanePonControlAbs()
        {
            this.weakReference = new(this);
            _createControls.Add(weakReference);
            // 下の値が設定されてないと例外が出るので初期値指定（しかも気が付きにくいエラー…）
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
            //ScaleChanged(1);
        }
    }
}
