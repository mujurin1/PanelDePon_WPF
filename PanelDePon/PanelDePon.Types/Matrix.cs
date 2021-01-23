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

        /// <summary>
        ///   自分自身と、自分+(+row, +col) したマトリックスを返す
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public (Matrix, Matrix) GetMatrixSet(int row, int col)
            => (this, new Matrix(this.Row + row, this.Column + col));

        public override string ToString()
            => $"Row: {Row}  Column: {Column}";
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
                if(RowLimitLower <= value && value <= RowLimitUpper)
                    _row = value;
            }
        }
        private int _column;
        public int Column {
            get => _column;
            set {
                // 範囲チェック
                if(ColumnLimitLower <= value && value <= ColumnLimitUpper)
                    _column = value;
            }
        }

        /// <summary>Rowがとり得る上限</summary>
        public int RowLimitUpper;
        /// <summary>Rowがとり得る下限</summary>
        public int RowLimitLower;
        /// <summary>Columnがとり得る上限</summary>
        public int ColumnLimitUpper;
        /// <summary>Columnがとり得る下限</summary>
        public int ColumnLimitLower;

        /// <summary>
        ///   <para>Row、Columnの上限はそれぞれareaSizeの値</para>
        ///   <para>下限はどちらも０</para>
        /// </summary>
        public MatrixRange(Matrix areaSize)
            : this(areaSize.Row, areaSize.Column) { }
        /// <summary>
        ///   <para>Row、Columnの上限はそれぞれrow、colの値</para>
        ///   <para>下限はどちらも０</para>
        /// </summary>
        public MatrixRange(int row, int col)
        {
            this._row = row;
            this._column = col;
            this.RowLimitLower = 0;
            this.RowLimitUpper = row;
            this.ColumnLimitLower = 0;
            this.ColumnLimitUpper = col;
        }

        public static implicit operator Matrix(MatrixRange _this)
            => new(_this.Row, _this.Column);

        /// <summary>
        ///   自分自身と、自分+(+row, +col) したマトリックスを返す
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public (Matrix, Matrix) GetMatrixSet(int row, int col)
            => (this, new Matrix(this.Row + row, this.Column + col));

        public override string ToString()
            => $"Row: {Row}  Column: {Column}";
    }
}
