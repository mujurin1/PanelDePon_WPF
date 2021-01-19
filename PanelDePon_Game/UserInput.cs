using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon_Game
{
    public struct UserInput
    {
        /// <summary>カーソルの位置</summary>
        public (int Row, int Col) CursorPos;
        /// <summary>
        ///   <para>この値が０なら、カーソルを動かせる</para>
        ///   <para>０でないなら１フレーム毎にマイナス１</para>
        /// </summary>
        public int CursorWait;

        public Input Input;
        public CursorState CursorState;
    }

    /// <summary>
    ///   ユーザー操作の種類
    /// </summary>
    public enum Input
    {
        Nan         = 0,        // 入力なし
        CursorUp    = 1,        // カーソル上移動
        CursorLeft  = 2,        // カーソル左移動
        CursorRight = 3,        // カーソル右移動
        CursorDown  = 4,        // カーソル下移動
        ChangeCell  = 5,        // セルを入れ替える
        ClickChangeCell = 6,    // クリックで入れ替える（カーソル移動＋入れ替え）
        Pause       = 7,        // ポーズ、まだ未実装
    }

    /// <summary>
    ///   カーソルの状態
    /// </summary>
    [Flags]
    public enum CursorState
    {
        Normar = 0,         // 操作可能。特に何もなし
        Wait   = 1 << 1,    // 操作不可能。アニメーション中？
    }
}
