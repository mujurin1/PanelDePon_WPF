﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PanelDePon_Game
{
    public struct RectangleArray<T>
    {
        private T[,] _array;

        public int Row => _array.GetLength(0);
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
            get => _array[row, col];
            set => _array[row, col] = value;
        }
    }
}
