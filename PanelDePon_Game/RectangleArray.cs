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

        public new T this[int row, int col] {
            get => _array[row, col];
            set => _array[row, col] = value;
        }
    }
}
