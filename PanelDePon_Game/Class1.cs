using System;
using System.Collections.ObjectModel;

namespace PanelDePon_Game
{
    public class Class1
    {
        /// <summary>
        ///   <para>セルの情報を格納した配列</para>
        ///   <para>一番下は、せり上がってるセル</para>
        /// </summary>
        private RectangleArray<CellInfo> _cellArray;
        /// <summary>セルの情報を格納した配列。読み取り専用</summary>
        public ReadonlyRectangleArray<CellInfo> CellArray;
        /// <summary>経過フレーム数</summary>
        public int ElapseFrame { get; set; } = 0;
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        /// <summary>セルがせり上がる速度</summary>
        public double UpSpeed { get; private set; }
        /// <summary>せり上がっている今の高さ</summary>
        public double Border{ get; private set; }
        /// <summary>Border がこの値まで来たら、完全にセルが上に移動する</summary>
        private static double BorderLine => 10;

        /// <summary>
        ///   ユーザーの入力と、カーソルの状態
        /// </summary>
        public UserInput UserInput { get; private set; }

        public Class1(int rows, int cols)
        {
            this.Rows = rows;
            this.Columns = cols;
            this._cellArray = new RectangleArray<CellInfo>(rows, cols);
            this.CellArray = _cellArray;
        }

        private void UpdateFrame()
        {

        }

        /* ゲームのセルの処理とか
         * １．操作
         * ２．下からセルがせり上がる（せり上がらないときもある）
         *     多分、ここでせり上がる時に一番上にセルがあればゲームオーバー
         * ３．左下から、右に、上に向かって、セルの状態変化（移動/変化）
         * ４．お邪魔セルの落下
         */
        private void GameRule()
        {

        }
    }
}
