using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon.Types
{
    /// <summary>
    ///   <para>セルの種類</para>
    ///   <para>空白：0x00</para>
    ///   <para>普通のセル：5bit目が１</para>
    ///   // 普通のセルの条件を変えるなら、NormalCellTypesとRandom()　を訂正してね！
    ///   <para>お邪魔：0x01 OR 0x02</para>
    /// </summary>
    public enum CellType
    {
        // 空白
        Empty     = 0x00,   // 0
        // 普通のセル
        Red       = 0x10,   // 16
        Blue      = 0x11,   // 17
        Sky       = 0x12,   // 18
        Yellow    = 0x13,   // 19
        Green     = 0x14,   // 20
        // びっくりマークのセル！
        // もしこのセルを消すなら、CellType.csの33行目辺りにあると思われる、NormalCellTypes　を訂正してね
        Bikkuri   = 0x1F,   // 31
        // お邪魔セル
        Ojama     = 0x01,   // 1
        // ハードお邪魔セル
        // ハードお邪魔が隣接してるなら、隣接してるハードは消える（ノーマルは消えない）  https://youtu.be/VrR5wGJAVmI?t=525
        HardOjama = 0x02    // 2
    }

    /// <summary>
    ///   ユーザーの操作
    /// </summary>
    public enum UserOperation
    {
        NaN = 0,                // 入力なし
        CursorUp = 1,           // カーソル上移動
        CursorLeft = 2,         // カーソル左移動
        CursorRight = 3,        // カーソル右移動
        CursorDown = 4,         // カーソル下移動
        ClickChangeCell = 5,    // クリックで入れ替える（カーソル移動＋入れ替え）、まだ未実装
        ChangeCell = 6,         // セルを入れ替える
        ScrollSpeedUp = 7,      // スクロール速度を早くする
    }

    /// <summary>
    ///   プレイエリアの状態
    /// </summary>
    public enum PlayAreaStatus
    {
        Initializing = 0,
        Ready = 1,
    }

    /// <summary>
    ///   セルの状態
    /// </summary>
    [Flags]
    public enum CellState
    {
        Move = 0,   // 動く　　（落下するし、入れ替えれる）
        Stop = 1,   // 動かない（落下も入れ替えもできない）
    }

    ///// <summary>
    /////   お邪魔だった場合、お邪魔のどの位置か
    ///// </summary>
    //public enum OjamaPart
    //{
    //    NaN = 0x00,     // お邪魔じゃない
    //    // １列のお邪魔
    //    One1 = 0x11,    // 左端
    //    One2 = 0x12,    // 中
    //    One3 = 0x13,    // 右端
    //    // ２列のお邪魔
    //    Two1 = 0x21,    // 左上
    //    Two2 = 0x22,    // 中上 32
    //    Two3 = 0x23,    // 右上
    //    Two4 = 0x24,    // 左下
    //    Two5 = 0x25,    // 中下 38
    //    Two6 = 0x26,    // 右下
    //    // ３列のお邪魔
    //    Thr1 = 0x31,    // 左上角
    //    Thr2 = 0x32,    // 中上 22
    //    Thr3 = 0x33,    // 右上角
    //    Thr4 = 0x34,    // 左
    //    Thr5 = 0x35,    // 中心
    //    Thr6 = 0x36,    // 右
    //    Thr7 = 0x37,    // 左下角
    //    Thr8 = 0x38,    // 中下 25
    //    Thr9 = 0x39,    // 右下角
    //}
}
