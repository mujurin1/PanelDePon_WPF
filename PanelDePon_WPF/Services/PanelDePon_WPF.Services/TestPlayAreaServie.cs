using PanelDePon.PlayArea;
using PanelDePon.Types;
using PanelDePon_WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon_WPF.Services
{
    public class TestPlayAreaServie : IPanelDePonPlayAreaService
    {
        /// <summary>経過フレーム数</summary>
        public int ElapseFrame { get; private set; }
        /// <summary>プレイエリアの広さ。row, col</summary>
        public Matrix _playAreaSize;
        public Matrix PlayAreaSize => _playAreaSize;
        /// <summary>
        ///   <para>プレイエリア内のセルの配列</para>
        ///   <para>注意）せり上がってくるセルは Row:-1</para>
        /// </summary>
        private RectangleArray<CellInfo> _cellArray;
        public RectangleArray<CellInfo> CellArray => _cellArray;
        /// <summary>今のスクロール位置</summary>
        public double ScrollLine { get; private set; }
        /// <summary>完全に１段スクロールする位置</summary>
        public double BorderLine { get; private set; }
        /// <summary>スクロールしてる割合</summary>
        public double ScrollPer { get; private set; }
        public bool PushedUp { get; private set; }
        /// <summary>カーソルの状態</summary>
        public CursorStatus _cursorStatus;
        public CursorStatus CursorStatus => _cursorStatus;
        /// <summary>CellArray に対応した、移動したセルの移動先</summary>
        public RectangleArray<Matrix?> _swapArray;
        public RectangleArray<Matrix?> SwapArray => _swapArray;
        /// <summary>ゲームオーバーしたかどうか</summary>
        public bool IsGameOver { get; private set; } = false;

        /// <summary>プレイエリアの更新が全て終了した時に呼ばれる</summary>
        public event EventHandler Updated;

        public TestPlayAreaServie()
        {
            // 固定値
            this._playAreaSize = new Matrix(12, 6);
            this.BorderLine = 100;
            // 更新ごとに変動  簡単な値の変化
            this.ElapseFrame = 0;
            this.ScrollLine = 0;
            this.ScrollPer = 0;
            // 更新ごとに変動  大事な値
            this._cellArray = new RectangleArray<CellInfo>(18, 6);
            this._cursorStatus = new CursorStatus(PlayAreaSize.Row, PlayAreaSize.Column-1);
            _cursorStatus.Matrix.Row = 0;
            _cursorStatus.Matrix.Column = 0;
            this._swapArray = new RectangleArray<Matrix?>(CellArray.Matrix);
            // セルの初期化
            _cellArray[0, 0] = RedCell;
        }

        /// <summary>プレイエリアを１フレーム分更新する</summary>
        /// <param name="userOperation">ユーザーの操作</param>
        public void InputKey(UserOperation userOperation)
        {
            // 更新時の最初の処理  フレーム１増加
            ElapseFrame++;

            /* ゲームアップデートの順序
             * 1. ユーザーの操作
             *    * カーソル位置移動
             *    * セルの入れ替え
             * 2. スクロール
             *    * IF ちょうど１段上がったなら、
             *      * セルを全て１段上に上げる
             *      * Row:-1 の段にセルを生成する
             *      * カーソルの位置を１段上に上げる
             * 3. セルの状態の更新
             *    1. 全てのセルの状態を更新する
             *       * StateTimer のカウントを減らす
             *    2. セルの落下
             *    3. 消滅するセルの検査
             *       1. 消滅するセルは、StateTiemr をセットする
             *       2. お邪魔とかいろいろ…
             */
            // ユーザーの操作
            _cursorStatus = CursorStatus.Update(userOperation);
            // スクロール
            ScrollLine += 2;        // スクロールする
            if(PushedUp = ScrollLine >= BorderLine) {    // ちょうど１段スクロールした
                ScrollLine = 0;     // スクロールラインを戻す
                s++;
                // 全てのセルを１段上げる
                for(var row = PlayAreaSize.Row - 2; row >= -1; row--)
                    for(var col = 0; col < PlayAreaSize.Column; col++)
                        _cellArray[row + 1, col] = CellArray[row, col];
                // カーソルを１段上げる
                // カーソルを１段上に上げる
                var cr = CursorStatus; var mr = cr.Matrix;
                mr.Row++;   // ホントはここがしたいだけ
                cr.Matrix = mr; _cursorStatus = cr;
            }
            ScrollPer = ScrollLine / BorderLine;    // 現在のスクロール割合
            _swapArray[s, 0] = null;
            _swapArray[s, 1] = null;
            // セルの状態更新
            if(B) { // 左から右
                _cellArray[s, 0] = _cellArray[s, 0].Update();
                if(_cellArray[s, 0].StateTimer.Lock == 0) {
                    var cell = _cellArray[s, 0];
                    cell.StateTimer = (0, 0, 10);
                    cell.Status = CellState.Lock;
                    _cellArray[s, 1] = cell;
                    _swapArray[s, 0] = new(s, 1);
                    B = false;
                }
            } else {// 右から左
                _cellArray[s, 1] = _cellArray[s, 1].Update();
                if(_cellArray[s, 1].StateTimer.Lock == 0) {
                    var cell = _cellArray[s, 1];
                    cell.StateTimer = (0, 0, 10);
                    cell.Status = CellState.Lock;
                    _cellArray[s, 0] = cell;
                    _swapArray[s, 1] = new(s, 0);
                    B = true;
                }
            }

            // 最後に、更新したよ～って言う
            Updated(this, EventArgs.Empty);
        }
        private bool B = true;
        private int s = 0;


        private CellInfo RedCell => GetCellInfo(CellType.Red);
        private CellInfo BlueCell => GetCellInfo(CellType.Blue);
        private CellInfo SkyCell => GetCellInfo(CellType.Sky);
        private CellInfo YellowCell => GetCellInfo(CellType.Yellow);
        private CellInfo GreenCell => GetCellInfo(CellType.Green);
        private CellInfo BikkuriCell => GetCellInfo(CellType.Bikkuri);

        private CellInfo GetCellInfo(CellType type)
        {
            var cell = CellInfo.Empty;
            cell.CellType = type;
            return cell;
        }
    }
}
