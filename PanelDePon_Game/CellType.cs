using PanelDePon_Game.Enums;
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
        /// <summary>お邪魔セルの部位。お邪魔じゃないなら NaN</summary>
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
        ///   <para>CellType列挙型の、普通のセルの数</para>
        ///   <para>びっくりマークのセルを含まないのでマイナス１</para>
        ///   <para>CellType列挙型に強く依存するので、CellType列挙型の変更に注意する</para>
        /// </summary>
        private static int NormalCellTypes = Enum.GetValues<CellType>()
                                                 .Select(e => (int)e)
                                                 .Where(n => (n & 0x10) == 0x10).Count() -1;

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

        private static Random _random = new Random();
        /// <summary>
        ///   <para>ランダムなセルを生成する</para>
        ///   <para>普通のセルのみ生成される</para>
        /// </summary>
        public static CellInfo Random()
        {
            /* 生成されるセル
             * 普通のセルの条件：5bit目が1
             * CellType 0x10 ~ 0x14 (16 ~ 20)
             */
            return new CellInfo() {
                CellType = (CellType)_random.Next(0x10, 0x10+NormalCellTypes)
            };
        }
    }

}