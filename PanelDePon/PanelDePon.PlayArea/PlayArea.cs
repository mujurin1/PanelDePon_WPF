using PanelDePon.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace PanelDePon.PlayArea
{
    public class PlayArea
    {
        /// <summary>
        ///   セルの入れ替えに掛かる時間
        /// </summary>
        private static int swapTime = 5;

        /// <summary>
        ///   <para>セルの情報を格納した配列</para>
        ///   <para>一番下の列は、せり上がってるセル</para>
        ///   <para>左下が row:0 col:0 で右上に行くほど増加</para>
        /// </summary>
        private RectangleArray<CellInfo> _cellArray;
        /// <summary>_cellArrayの公開用。値型のプロパティなので、変更は不可能</summary>
        public RectangleArray<CellInfo> CellArray => _cellArray;

        /// <summary>経過フレーム数</summary>
        public int ElapseFrame { get; private set; } = 0;
        /// <summary>プレイエリアの広さ。row, col</summary>
        public Matrix PlayAreaSize { get; private set; }

        /* スクロールをするかどうかメモ
         * 
         * 下の動作中は、スクロールが止まる
         * ・お邪魔が消えているとき
         * ・セルが消えているとき
         * 下の動作を起こすと、一定時間スクロールが止まる
         * ・一定以上の数を同時消ししたとき
         * ・コンボはどうだろう？
         * 
         * 
         */
        /// <summary>
        ///   <para>スクロール待機フレーム数。０ならスクロールする</para>
        ///   <para>セルの状態変更時に、WaitFrame が最も高い値がここの値になる　多分それでいい</para>
        /// </summary>
        private int _scrollWaitFrame = 0;
        /// <summary>スクロールする速度</summary>
        public double ScrollSpeed { get; set; } = 2;      // てきとう
        /// <summary>せり上がっている今の高さ</summary>
        public double ScrollLine { get; private set; } = 0;
        /// <summary>Border がこの値まで来たら、完全にセルが上に移動する</summary>
        public readonly double BorderLine = 100;          // てきとう

        /// <summary>
        ///   カーソルの状態
        /// </summary>
        public CursorStatus CursorStatus { get; private set; }

        private RectangleArray<Matrix?> _swapArray;
        /// <summary>
        ///   <para>移動したセルの位置に、移動先の座標が入っている。移動してないセルは null</para>
        /// </summary>
        public RectangleArray<Matrix?> SwapArray => _swapArray;
        /// <summary>プレイエリアの更新が終わっているか</summary>
        public bool Updating { get; private set; }
        /// <summary>ゲームオーバーしたかどうか</summary>
        public bool IsGameOver { get; private set; } = false;

        /// <summary>
        ///   プレイエリアの更新が全て終了した時に呼ばれる
        /// </summary>
        public event EventHandler Updated;


        public PlayArea(int row, int col)
        {
            this.PlayAreaSize = new Matrix(row, col);
            // セルの存在する２次元配列
            // rows（縦列）はお邪魔が降って来て画面上範囲外にセルが存在するので、1.5倍にして、
            // 　　　　　　　一番下はせり上がる列なので更に＋１
            this._cellArray = new RectangleArray<CellInfo>((int)(row * 1.5) + 1, col);
            this._swapArray = new RectangleArray<Matrix?>(CellArray.Row, CellArray.Column);
            this.CursorStatus = new CursorStatus(PlayAreaSize.Row, PlayAreaSize.Column - 1);
            PlayAreaInit();
        }


        /// <summary>
        ///   <para>プレイエリアの状態を初期化する</para>
        /// </summary>
        private void PlayAreaInit()
        {
            // カーソル位置を初期化
            var cursor = CursorStatus;
            cursor.CursorPos.Row = 0;
            cursor.CursorPos.Column = 0;
            CursorStatus = cursor;
            // セルの配置を初期化する
            _cellArray = CellArray.CopyAndReset(CellInfo.Empty);
            for(int row = -1; row <= 5; row++) {
                for(int col = 0; col < PlayAreaSize.Column; col++) {
                    var cell = CellArray[row, col];
                    cell.CellType = CellInfo.RandomCellType(n: -1);
                    _cellArray[row, col] = cell;
                }
            }
            //==============================Debug用
            //var dbC = CellInfo.Empty;
            //dbC.CellType = CellType.Yellow;
            //_cellArray[0, 0] = dbC;
            //_cellArray[0, 1] = dbC;
            //_cellArray[0, 2] = dbC;
            //_cellArray[0, 3] = dbC;
            //_cellArray[0, 4] = dbC;
            //_cellArray[0, 5] = dbC;
            //// 一番下（Row-1）のセルをランダムに追加する
            //for(int col = 0; col < PlayAreaSize.Column; col++) {
            //    var cell = CellArray[-1, col];
            //    cell.CellType = CellInfo.RandomCellType(n: 0);
            //    _cellArray[-1, col] = cell;
            //}
            //==============================Debug用 ここまで

            //var ojama = new CellInfo {
            //    CellType = CellType.Ojama,
            //};

            // スクロール位置の初期化
            ScrollLine = 0;

            // スクロール速度の初期化
            // せり上がっている今の高さの初期化
            // （まだフィールドすら無いけど）お邪魔の初期化
        }

        private readonly object _updateLock = new object();
        /// <summary>
        ///   プレイエリアを１フレーム分更新する
        /// </summary>
        /// <param name="userOperation">ユーザーの操作</param>
        public void UpdateFrame(UserOperation userOperation)
        {
            if(IsGameOver) {
                Debug.WriteLine("ゲームオーバーしてます");
                return;
            }
            // 更新中かどうか
            lock(_updateLock) {
                if(Updating) {
                    Debug.WriteLine("更新中に次の更新が呼ばれた");
                    return;
                }
                Updating = true;
            }
            GameRule(userOperation);

            Updating = false;
            // 一番最後に実行される
            Updated?.Invoke(this, EventArgs.Empty);
        }

        /* プレイエリアのセルの処理とか
         * １．操作
         * ２．スクロールする（しないときもある）
         *     　ここでせり上がる時に一番上にセルがあればゲームオーバー
         * ３．左下から、右に、上に向かって、セルの状態変化（移動/変化）
         * ４．お邪魔セルの落下
         */
        /// <summary>
        ///   ゲームの状態を１フレーム更新する
        /// </summary>
        /// <param name="userOperation">ユーザーの操作</param>
        private void GameRule(UserOperation userOperation)
        {
            // 移動したセル配列の初期化
            _swapArray = SwapArray.CopyAndReset(null);

            //==============================ユーザーの操作==============================
            switch(userOperation) {
            case UserOperation.Swap:      // カーソルの位置のセルを入れ替える
                // カーソルの位置のセルを取得
                var cursorCellL = CellArray[CursorStatus.CursorPos.Row, CursorStatus.CursorPos.Column];
                var cursorCellR = CellArray[CursorStatus.CursorPos.Row, CursorStatus.CursorPos.Column + 1];

                // 両方Emptyなら入れ替える必要がないので入れ替えない
                if(cursorCellL.CellType is CellType.Empty && cursorCellR.CellType is CellType.Empty)
                    break;

                // カーソル位置のセルのどちらも移動可能
                if(cursorCellL.CellType.IsNomal() && cursorCellL.Status is CellState.Free &&
                   cursorCellR.CellType.IsNomal() && cursorCellR.Status is CellState.Free) {
                    SwapCell(CursorStatus.CursorPos.Row, CursorStatus.CursorPos.Column,
                             CursorStatus.CursorPos.Row, CursorStatus.CursorPos.Column + 1);
                    // 入れ替わりリストに追加
                    //swapList.Add(mat);
                }
                break;
            case UserOperation.ScrollSpeedUp:
                // TODO: スクロール速度が上昇する
                break;
            default:
                break;
            }
            // カーソルの状態を更新する
            CursorStatus = CursorStatus.Update(userOperation);

            //==============================スクロールする==============================
            // スクロール待機中なら、待機フレーム数を１減らしてスクロールしない
            if(_scrollWaitFrame > 0) {
                _scrollWaitFrame--;
            } else {
                // スクロールする
                ScrollLine += ScrollSpeed;
                // ゲームオーバーか？
                if(GameOverJudge()) {
                    // TODO: ゲームオーバー
                    IsGameOver = true;
                    return;
                }
                // ScrollLine が BorderLine を超えているか？（１段上に上げるか？）
                if(ScrollLine >= BorderLine) {
                    ScrollLine = 0;
                    int row;
                    // セルを１段上に上げる
                    for(row = PlayAreaSize.Row - 2; row >= -1; row--) {
                        for(var col = 0; col < PlayAreaSize.Column; col++) {
                            _cellArray[row + 1, col] = CellArray[row, col];
                        }
                    }
                    // カーソルを１段上に上げる
                    var cr = CursorStatus;
                    var mr = cr.CursorPos;
                    mr.Row++;   // ホントはここがしたいだけ
                    cr.CursorPos = mr;
                    CursorStatus = cr;
                    // 一番下（Row-1）のセルをランダムに追加する
                    for(int col = 0; col < PlayAreaSize.Column; col++) {
                        var cell = CellArray[-1, col];
                        cell.CellType = CellInfo.RandomCellType(n: 0);
                        _cellArray[-1, col] = cell;
                    }
                }
            }

            //==============================セルの状態を更新する==============================
            /* ・更新順序
             * 1. セルの状態更新
             * 2. 今回のフレームで入れ替わる
             *    (入れ替え or 落下。入れ替わったセルは SwapArray の対応した場所にインデックスが入る)
             * 3. セルの消滅、変身
             *    * 消滅セルを検査する
             *    * 変身するお邪魔セルを検査する
             *    * 消滅、変身するセルの状態をセットする
             */
            //======================セルの状態更新
            //========全てのセルを更新
            for(int row = 0; row < CellArray.Row; row++) {
                for(int col = 0; col < CellArray.Column; col++) {
                    var cell = CellArray[row, col];
                    cell.Update();
                    _cellArray[row, col] = cell;
                }
            }
            //==========セルの落下
            for(int row = 1; row < CellArray.Row; row++) {
                for(int col = 0; col < CellArray.Column; col++) {
                    // TODO: お邪魔の落下

                    // 自分のセルが Not(種類:ノーマル or 状態:Free)
                    if(!CellArray[row, col].CellType.IsNomal() || CellArray[row, col].Status is not CellState.Free)
                        continue;

                    var bottomCell = CellArray[row - 1, col];
                    // 下のセルが 種類:空 かつ 状態:Free
                    if(bottomCell.CellType is CellType.Empty && bottomCell.Status is CellState.Free) {
                        // 自分と下のセルを入れ替えて、落下を起こす
                        SwapCell(row, col, row - 1, col, isSwap: false);
                    }
                }
            }
            //==========================セルの消滅、変身
            int flashTime = 10;     // セルがフラッシュし続けるフレーム数
            int momentTime = 2;     // セルが消滅変身するアニメーションに掛かる時間

            var listD = new List<(int row, int col, CellInfo)>();   // お邪魔。変身するセルを格納する

            var searchAry = new RectangleArray<bool>(PlayAreaSize);
            // 横列走査
            var funcRow = new Func<Matrix, Matrix>(m => { m.Column++; return m; });
            for(int row = 0; row < searchAry.Row; row++)
                Serch(res:ref searchAry, func:funcRow, beforeCell:CellInfo.Empty, chain:0, new Matrix(row, 0));
            // 縦列走査
            var funcCol = new Func<Matrix, Matrix>(m => { m.Row++; return m; });
            for(int col = 0; col < searchAry.Column; col++)
                Serch(res:ref searchAry, func:funcCol, beforeCell:CellInfo.Empty, chain:0, new Matrix(0, col));


            // TODO: 変身するお邪魔セル走査


            //// 重複を取り除いて、row の降順 col の昇順に並び替え
            //listC = listC.Distinct().OrderByDescending(c => c.row).ThenBy(c => c.col).ToList();

            // 消滅するセルを、消滅するように仕向ける
            int cnt = 0;                        // 消滅セル数    （左上から順番にカウント）
            int sum = searchAry.Count(b => b);  // 消滅セル数合計
            for(int row = searchAry.Row-1; row >= 0; row--) {
                for(int col = 0; col < searchAry.Column; col++) {
                    if(!searchAry[row, col]) continue;
                    var cell = CellArray[row, col];
                    // 状態遷移タイマーのセット
                    cell.StateTimer = (flashTime, cnt * momentTime + 2, (sum - cnt) * momentTime);
                    // セルの状態をフラッシュに
                    cell.Status = CellState.Flash;
                    _cellArray[row, col] = cell;
                    cnt++;
                }
            }

        }
        /// <summary>
        ///   繋がっているかを検査する
        /// </summary>
        /// <param name="res">結果</param>
        /// <param name="func">Row,Columnの値を増やすデリゲート</param>
        /// <param name="beforeType">１つ前に検査したセルの種類</param>
        /// <param name="chain">１つ前の連鎖数</param>
        /// <param name="now">今回調べる位置</param>
        /// <returns>３つ以上の繋がっているうちの１つだった</returns>
        bool Serch(ref RectangleArray<bool> res, Func<Matrix, Matrix> func, CellInfo beforeCell, int chain, Matrix now)
        {
            // 今回調べるセル
            var nowCell = CellArray[now];
            // 前回のセル or 今回のセルが消滅に使えない または 今回調べるセルと違うタイプ or 
            if(!beforeCell.IsEliminatable || !nowCell.IsEliminatable || nowCell.CellType != beforeCell.CellType)
                chain = 1;
            else
                chain++;

            var next = func(now);
            var isChain = false;
            if(next.Row < res.Row && next.Column < res.Column)
                isChain = Serch(ref res, func, nowCell, chain, next);
            else
                if(chain >= 3)
                    isChain = true; // return res[now] = true; 確定

            if(isChain || chain >= 3) {
                res[now] = isChain = true;
            } else {
                res[now] = res[now] || false;
            }
            return chain > 1 && isChain;
        }

        /// <summary>
        ///   <para>row,col の位置のセルを swapRow,swapCol の位置のセルと入れ替えて入れ替え配列にセット</para>
        ///   <para>落下なら、row, col が落下するセル</para>
        /// </summary>
        /// <param name="isSwap">その入れ替えは落下か？</param>
        /// <returns>((rox,col)のMatrix, swapのMatrix)</returns>
        private void SwapCell(int row, int col, int swapRow, int swapCol, bool isSwap = true)
        {
            // セルインフォの取得
            var cell = CellArray[row, col];
            var swapCell = CellArray[swapRow, swapCol];
            // 空のセルでなければ状態をLockに
            if(cell.CellType is not CellType.Empty) {
                cell.Status = CellState.Lock;
                cell.StateTimer = (0, 0, swapTime);
            }
            if(swapCell.CellType is not CellType.Empty) {
                swapCell.Status = CellState.Lock;
                swapCell.StateTimer = (0, 0, swapTime);
            }
            // 移動（入れ替え
            _cellArray[row, col] = swapCell;
            _cellArray[swapRow, swapCol] = cell;
            // 入れ替えた配列に追加
            _swapArray[row, col] = new Matrix(swapRow, swapCol);
            if(isSwap)
                _swapArray[swapRow, swapCol] = new Matrix(row, col);
        }

        /// <summary>
        ///   ゲームオーバー判定</para>
        /// </summary>
        private bool GameOverJudge()
        {
            return false;
        }
    }
}
