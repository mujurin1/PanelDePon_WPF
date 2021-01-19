using System;
using System.Collections.Generic;
using System.Text;

namespace PanelDePon_Game
{
    public class ReadonlyRectangleArray<T>
    {
        protected T[,] _array;

        public readonly int Width;
        public readonly int Heihgt;

        protected ReadonlyRectangleArray() { }

        public T this[int row, int col] {
            get => _array[row, col];
        }
    }
}
