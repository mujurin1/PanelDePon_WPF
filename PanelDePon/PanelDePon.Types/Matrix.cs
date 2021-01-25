using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon.Types
{
    /// <summary>
    ///   Row（縦）Column（横）の２つの値を持つレコード
    /// </summary>
    public struct Matrix
    {
        public int Row;
        public int Column;

        public Matrix(int row, int col)
        {
            this.Row = row;
            this.Column = col;
        }

        public override string ToString()
            => $"Row: {Row}  Column: {Column}";

        public bool Equals(Matrix matrix)
            => this.Row == matrix.Row && this.Column == matrix.Column;
    }

    /// <summary>
    ///   <para>Row（縦）Column（横）の２つの値と、</para>
    ///   <para>Row、Column の上限、下限を持つレコード</para>
    /// </summary>
    public struct MatrixRange
    {
        private int _row;
        public int Row {
            get => _row;
            set {
                // 範囲チェック
                if(RowLimit.Lower <= value && value < RowLimit.Upper)
                    _row = value;
            }
        }
        private int _column;
        public int Column {
            get => _column;
            set {
                // 範囲チェック
                if(ColumnLimit.Lower <= value && value < ColumnLimit.Upper)
                    _column = value;
            }
        }

        /// <summary>
        ///   <para>Rowがとり得る範囲（下限,上限）</para>
        ///   <para>下限以上、上限未満</para>
        /// </summary>
        public (int Lower, int Upper) RowLimit;
        /// <summary>
        ///   <para>Columnがとり得る範囲（下限,上限）</para>
        ///   <para>下限以上、上限未満</para>
        /// </summary>
        public (int Lower, int Upper) ColumnLimit;

        public MatrixRange(int row, int col) : this(new Matrix(row, col)) { }
        /// <summary>
        ///   <para>渡された Matrix を自分の位置、範囲の上限にする</para>
        ///   <para>下限の初期値は０</para>
        /// </summary>
        public MatrixRange(Matrix matrix)
        {
            this._row = matrix.Row;
            this._column = matrix.Column;
            this.RowLimit = (0, matrix.Row);
            this.ColumnLimit = (0, matrix.Column);
        }

        public static implicit operator Matrix(MatrixRange _this)
            => new(_this.Row, _this.Column);

        public override string ToString()
            => $"Row: {Row}  Column: {Column}";
    }
}
