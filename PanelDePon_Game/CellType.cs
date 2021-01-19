using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon_Game
{
    /// <summary>
    ///   そのマスのセルの情報
    /// </summary>
    public struct CellInfo
    {
        /// <summary>セルの種類</summary>
        public CellType CellType;
        /// <summary>お邪魔セルの部位。お邪魔じゃないなら０</summary>
        public OjamaPart OjamaPart;
        /// <summary>セルの状態。動、落</summary>
        public CellState State;
        /// <summary>
        ///   この値が０になると、セルの情報をNextStateに変更する
        /// </summary>
        public int WaitFrame;
        /// <summary>
        ///   セルの次の情報。nフレーム後にこの情報に更新
        /// </summary>
        public CellState NextState;

        /// <summary>
        ///   １フレーム経過
        /// </summary>
        public void Update()
        {
            // 待機中でないなら何もしない
            if(WaitFrame == 0) return;
            // 残り１フレームで状態変化するなら、それは今
            if(WaitFrame == 1) {
                this.State = NextState;
            }

            // フレームを１経過させる
            WaitFrame--;
        }
    }

    /// <summary>
    ///   <para>セルの種類</para>
    ///   <para>空白：0x00</para>
    ///   <para>普通のセル：5bit目が１</para>
    ///   <para>お邪魔：0x01 OR 0x02</para>
    /// </summary>
    public enum CellType
    {
        // 空白
        NaN       = 0x00,   // 0
        // 普通のセル
        Red       = 0x10,   // 16
        Blue      = 0x11,   // 17
        Sky       = 0x12,   // 18
        Yellow    = 0x13,   // 19
        Green     = 0x14,   // 20
        // ハテナ？
        Hatena    = 0x1F,   // 31
        // お邪魔
        Ojama     = 0x01,   // 1
        // ハードお邪魔
        HardOjama = 0x02    // 2
    }
    /// <summary>
    ///   セルの状態
    /// </summary>
    [Flags]
    public enum CellState
    {
        Shift = 1,          // 動かせる
        Fall  = 1 << 1,     // 落下可能
    }
    /// <summary>
    ///   お邪魔だった場合、お邪魔のどの位置か
    /// </summary>
    public enum OjamaPart
    {
        NaN  = 0x00,    // お邪魔じゃない
        // １列のお邪魔
        One1 = 0x11,    // 左端
        One2 = 0x12,    // 中
        One3 = 0x13,    // 右端
        // ２列のお邪魔
        Two1 = 0x21,    // 左上
        Two2 = 0x22,    // 中上 32
        Two3 = 0x23,    // 右上
        Two4 = 0x24,    // 左下
        Two5 = 0x25,    // 中下 38
        Two6 = 0x26,    // 右下
        // ３列のお邪魔
        Thr1 = 0x31,    // 左上角
        Thr2 = 0x32,    // 中上 22
        Thr3 = 0x33,    // 右上角
        Thr4 = 0x34,    // 左
        Thr5 = 0x35,    // 中心
        Thr6 = 0x36,    // 右
        Thr7 = 0x37,    // 左下角
        Thr8 = 0x38,    // 中下 25
        Thr9 = 0x39,    // 右下角
    }
}