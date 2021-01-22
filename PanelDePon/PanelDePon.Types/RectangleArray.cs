using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PanelDePon.Types
{
    public struct RectangleArray<T>
    {
        private readonly T[,] _array;

        public int Row => _array.GetLength(0) - 1;
        public int Column => _array.GetLength(1);

        public RectangleArray(int row, int col)
        {
            this._array = new T[row + 1, col];
        }

        /// <summary>
        ///   <para>配列の取得、変更</para>
        ///   <para>今回のゲームの都合上、Row:-1 で一番下の列にアクセスする</para>
        /// </summary>
        public T this[int row, int col] {
            get => _array[row + 1, col];
            set => _array[row + 1, col] = value;
        }

        /// <summary>
        ///   配列を全て item で埋める
        /// </summary>
        /// <param name="item">この値で埋める</param>
        public void Reset(T item)
        {
            for(int row = -1; row < Row; row++)
                for(int col = 0; col < Column; col++) {
                    this[row, col] = item;
                }
        }
    }
}
