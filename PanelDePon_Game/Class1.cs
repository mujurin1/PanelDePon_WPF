using System;
using System.Collections.ObjectModel;

namespace PanelDePon_Game
{
    public class Class1
    {
        /// <summary>
        ///   セルの配列（長方形の配列）
        /// </summary>
        private RectangleArray<object> _cellArray;
        /// <summary>
        ///   セルの配列。読み取り専用
        /// </summary>
        public ReadonlyRectangleArray<object> CellArray;
        
        public Class1(int rows, int cols)
        {
            this._cellArray = new RectangleArray<object>(rows, cols);
            this.CellArray = _cellArray;
        }
    }
}
