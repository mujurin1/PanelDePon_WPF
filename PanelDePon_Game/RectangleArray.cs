using System;
using System.Collections.Generic;
using System.Text;

namespace PanelDePon_Game
{
    public class RectangleArray<T> : ReadonlyRectangleArray<T>
    {
        public RectangleArray(int rows, int cols)
        {
            this._array = new T[rows, cols];
        }

        /// <summary>
        ///   <para>配列の取得、変更</para>
        ///   <para>今回のゲームの都合上、Row:-1 で一番下の列にアクセスする</para>
        /// </summary>
        public new T this[int row, int col] {
            get => _array[row+1, col];
            set => _array[row+1, col] = value;
        }
    }
}
