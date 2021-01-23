using PanelDePon.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon_WPF.Services.Interfaces
{
    /// <summary>
    ///   PanelDePon.PlayAreaクラスの情報と操作を提供するインターフェース
    /// </summary>
    public interface IPanelDePonPlayAreaService
    {
        /// <summary>経過フレーム数</summary>
        int ElapseFrame { get; }
        /// <summary>プレイエリアの広さ。row, col</summary>
        Matrix PlayAreaSize { get; }
        /// <summary>
        ///   <para>プレイエリア内のセルの配列</para>
        ///   <para>注意）せり上がってくるセルは Row:-1</para>
        /// </summary>
        RectangleArray<CellInfo> CellArray { get; }
        /// <summary>今のスクロール位置</summary>
        double ScrollLine { get; }
        /// <summary>完全に１段スクロールする位置</summary>
        double BorderLine { get; }
        /// <summary>カーソルの状態</summary>
        CursorStatus CursorStatus { get; }

        /// <summary>プレイエリアを１フレーム分更新する</summary>
        /// <param name="userOperation">ユーザーの操作</param>
        void UpdateFrame(UserOperation userOperation);

        /// <summary>プレイエリアの更新が全て終了した時に呼ばれる</summary>
        event UpdateEventHandler Updated;
    }
}
