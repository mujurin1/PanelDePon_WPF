using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PanelDePon.Types
{
    public struct RectangleArray<T> : IEnumerable<T>
    {
        private readonly T[,] _array;

        public readonly int Row;
        public readonly int Column;
        public readonly Matrix Matrix;

        public RectangleArray(int row, int col) : this(new Matrix(row, col)) { }
        public RectangleArray(Matrix matrix)
        {
            this._array = new T[matrix.Row + 1, matrix.Column];
            this.Row = matrix.Row;
            this.Column = matrix.Column;
            this.Matrix = matrix;
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
        ///   Matrix をインデックスにしてアクセスする
        /// </summary>
        public T this[Matrix matrix] {
            get => this[matrix.Row, matrix.Column];
            set => this[matrix.Row, matrix.Column] = value;
        }

        /// <summary>
        ///   自身のコピーを、全て item で埋めて返す
        /// </summary>
        /// <param name="item">この値で埋める</param>
        public RectangleArray<T> CopyAndReset(T item)
        {
            RectangleArray<T> copy = new RectangleArray<T>(this.Row, this.Column);
            for(int row = -1; row < Row; row++)
                for(int col = 0; col < Column; col++)
                    copy[row, col] = item;
            return copy;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int row = -1; row < Row; row++) {
                for(int col = 0; col < Column; col++) {
                    yield return this[row, col];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
