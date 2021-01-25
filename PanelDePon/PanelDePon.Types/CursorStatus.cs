using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon.Types
{
    /// <summary>
    ///   ユーザーが操作するカーソルの状態
    /// </summary>
    public struct CursorStatus
    {
        /// <summary>
        ///   <para>プレイエリアサイズ（カーソルが存在出来る範囲）</para>
        /// </summary>
        public Matrix PlayAreaSize;
        /// <summary>
        ///   <para>カーソルの位置（左側）</para>
        ///   <para>左下が row:0 col:0 で右上に行くほど増加</para>
        /// </summary>
        public MatrixRange Matrix;
        /// <summary>
        ///   <para>この値が０なら、カーソルの操作や入れ替えができる</para>
        ///   <para>０でないなら１フレーム毎にマイナス１</para>
        /// </summary>
        public uint CursorWait;

        public CursorStatus(int row, int col) : this (new Matrix(row, col)) { }
        public CursorStatus(Matrix playAreaSize)
        {
            this.PlayAreaSize = playAreaSize;
            this.Matrix = new MatrixRange(playAreaSize);
            this.CursorWait = 0;
        }

        /// <summary>
        ///   <para>カーソルの状態を１フレーム更新する</para>
        ///   <para>userOperation は、カーソル操作系しかあり得ない</para>
        /// </summary>
        /// <param name="userOperation">ユーザーの操作</param>
        public CursorStatus Update(UserOperation userOperation)
        {
            // カーソルが操作不能
            if(CursorWait > 0) {
                CursorWait--;
                return this;
            }
            switch(userOperation) {
            case UserOperation.NaN or 
                 UserOperation.ClickChangeCell or
                 UserOperation.Swap or
                 UserOperation.ScrollSpeedUp:
                break;
            case UserOperation.CursorUp:
                CursorMove(1, 0);
                break;
            case UserOperation.CursorLeft:
                CursorMove(0, -1);
                break;
            case UserOperation.CursorRight:
                CursorMove(0, 1);
                break;
            case UserOperation.CursorDown:
                CursorMove(-1, 0);
                break;
            }
            return this;
        }

        private void CursorMove(int row, int column)
        {
            var matrix = Matrix;
            matrix.Row += row;
            matrix.Column += column;
            Matrix = matrix;
        }
    }
}
