using PanelDePon.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PanelDePon.PlayArea
{
    public class PlayArea
    {
        /// <summary>
        ///   セルが消滅するかの処理で使う。_cellArrayと同じサイズ
        /// </summary>
        private RectangleArray<CellInfo> _eliminateArray;
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
        public AreaSize PlayAreaSize { get; private set; }

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
        private uint _scrollWaitFrame = 0;
        /// <summary>スクロールする速度</summary>
        public uint ScrollSpeed { get; set; } = 2;      // てきとう
        /// <summary>せり上がっている今の高さ</summary>
        public uint ScrollLine { get; private set; } = 0;
        /// <summary>Border がこの値まで来たら、完全にセルが上に移動する</summary>
        private static readonly uint BorderLine = 100; // てきとう

        /// <summary>
        ///   カーソルの状態
        /// </summary>
        public CursorStatus CursorStatus { get; private set; }
        /// <summary>
        ///   動かしたセルのリスト。[0:(OldRow, OldCol), 1:(NewRow, NewCol)]
        /// </summary>
        private readonly List<(int Row, int Column)[]> _moveCellList = new List<(int Row, int Column)[]>();
        /// <summary>
        ///   動かしたセルの読み取り専用リスト。[0:(OldRow, OldCol), 1:(NewRow, NewCol)]
        /// </summary>
        public IReadOnlyList<(int Row, int Column)[]> MoveCellList => _moveCellList;

        /// <summary>
        ///   プレイエリアの更新が全て終了した時に呼ばれる
        /// </summary>
        public event EventHandler Updated;
        /// <summary>
        ///   ゲームオーバーしたときに呼ばれる
        /// </summary>
        public event EventHandler GameOver;


        public PlayArea(int row, int col)
        {
            this.PlayAreaSize = new AreaSize(row, col);
            // セルの存在する２次元配列
            // rows（縦列）はお邪魔が降って来て画面上範囲外にセルが存在するので、1.5倍にして、
            // 　　　　　　　一番下はせり上がる列なので更に＋１
            this._cellArray = new RectangleArray<CellInfo>((int)(row * 1.5) + 1, col);
            this._eliminateArray = new RectangleArray<CellInfo>(_cellArray.Row, _cellArray.Column);
            this.CursorStatus = new CursorStatus(PlayAreaSize);
            PlayAreaInit();
        }

        /// <summary>
        ///   <para>プレイエリアの状態を初期化する</para>
        /// </summary>
        private void PlayAreaInit()
        {
            // セルの配置を初期化する
            _cellArray.Reset(CellInfo.Empty);
            _eliminateArray.Reset(CellInfo.Empty);
            for(int row = -1; row <= 5; row++) {
                for(int col = 0; col < PlayAreaSize.Column; col++) {
                    var cell = _cellArray[row, col];
                    cell.CellType = CellInfo.RandomCellType(n: -1);
                    _cellArray[row, col] = cell;
                }
            }

            //// Debug用に、お邪魔セルを生成する
            //var ojama = new CellInfo {
            //    CellType = CellType.Ojama,
            //};

            // スクロール位置の初期化
            ScrollLine = 0;

            // スクロール速度の初期化
            // せり上がっている今の高さの初期化
            // （まだフィールドすら無いけど）お邪魔の初期化
        }

        /// <summary>
        ///   プレイエリアを１フレーム分更新する
        /// </summary>
        /// <param name="userOperation">ユーザーの操作</param>
        public void UpdateFrame(UserOperation userOperation)
        {
            // 動かしたセルのリストの初期化　GameRule() よりも先に
            _moveCellList.RemoveAll(_ => true);
            GameRule(userOperation);

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
        private void GameRule(UserOperation userOperation)
        {
            //==============================ユーザーの操作==============================
            switch(userOperation) {
            case UserOperation.NaN:
                break;
            case >= UserOperation.CursorUp and <= UserOperation.ClickChangeCell:
                // カーソル移動系 or クリック操作なら、カーソルに処理を渡して、ごーとぅー
                CursorStatus.UpdateFrame(userOperation);
                goto CursorUpdateCompleted;
            case UserOperation.ChangeCell:      // カーソルの位置のセルを入れ替える
                // カーソルの位置のセルを取得
                var cursorCellL = _cellArray[CursorStatus.CursorPos.Row, CursorStatus.CursorPos.Column];
                var cursorCellR = _cellArray[CursorStatus.CursorPos.Row, CursorStatus.CursorPos.Column + 1];

                // 入れ替えに掛かる時間を仮に決めておく
                var lockTime = 5;

                // カーソル位置のセルのどちらも移動可能
                if(cursorCellL.CanMove && cursorCellR.CanMove) {
                    // 入れ替えてる最中はロックする
                    cursorCellL.Status = CellState.Lock;
                    cursorCellL.StateTimer = (0, 0, lockTime);
                    cursorCellR.Status = CellState.Lock;
                    cursorCellR.StateTimer = (0, 0, lockTime);
                    // MEMO: 値型だからこうやっても大丈夫だよね？
                    _cellArray[CursorStatus.CursorPos.Row, CursorStatus.CursorPos.Column] = cursorCellR;
                    _cellArray[CursorStatus.CursorPos.Row, CursorStatus.CursorPos.Column + 1] = cursorCellL;
                }
                break;
            case UserOperation.ScrollSpeedUp:
                // TODO: スクロール速度が上昇する
                break;
            }
            // カーソルの移動系以外の操作だった場合
            // カーソルの状態を更新するのは必須なので
            CursorStatus.UpdateFrame();

        CursorUpdateCompleted:  // カーソルの状態を更新したらここに来る
            

            //==============================スクロールする==============================
            // スクロール待機中なら、待機フレーム数を１減らしてスクロールしない
            if(_scrollWaitFrame > 0) {
                _scrollWaitFrame--;
            } else {
                // スクロールする
                ScrollLine += ScrollSpeed;
                // ゲームオーバーか？
                if(IsGameOver()) {
                    GameOver?.Invoke(this, EventArgs.Empty);
                    return;
                }
                // ScrollLine が BorderLine を超えているか？（１段上に上げるか？）
                if(ScrollLine >= BorderLine) {
                    ScrollLine = 0;
                    int row;
                    // セルを１段上に上げる
                    for(row = PlayAreaSize.Row - 2; row >= -1; row--) {
                        for(var col = 0; col < PlayAreaSize.Column; col++) {
                            _cellArray[row + 1, col] = _cellArray[row, col];
                        }
                    }
                    // 一番下（Row-1）のセルをランダムに追加する
                    for(int col = 0; col < PlayAreaSize.Column; col++) {
                        var cell = _cellArray[row, col];
                        cell.CellType = CellInfo.RandomCellType(n: 0);
                        _cellArray[row, col] = cell;
                    }
                }
            }

            //==============================セルの状態を更新する==============================
            // TODO: セルの状態を更新する
            int flashTime = 10;     // セルがフラッシュし続けるフレーム数
            int momentTime = 2;     // セルが消滅変身するアニメーションに掛かる時間

            var listC = new List<(int row, int col, CellInfo)>();   // 通常。　消滅するセルを格納する 位置が大事なので、row col も保持
            var listD = new List<(int row, int col, CellInfo)>();   // お邪魔。変身するセルを格納する

            // セルを走査する、デリゲート
            Action<int, int> scaning = new Action<int, int>((first, second) => {
                for(int fst = 0; fst < first; fst++) {
                    var checkCellType = CellArray[fst, 0].CellType;     // 前回見たセルの種類
                    var chainCnt = -1;                                  // 同じ種類のセルが何回続いたか。初期値が０なのは、後で範囲を取るとき楽するため
                    for(int scd = 1; scd < second; scd++) {
                        var nowCell = CellArray[fst, scd];
                        if(checkCellType.Equals(nowCell.CellType)) {    // 前回のセルと同じ種類
                            chainCnt++;
                        } else {                                        // 前回見たセルと違う種類
                            if(chainCnt >= 2)                           // 過去３つ以上同じだった
                                for(; chainCnt >= 0; chainCnt--)
                                    listC.Add(GetEliminateCell(fst, scd - chainCnt));
                            chainCnt = -1;
                        }
                    }
                }
            });
            // 横列走査
            scaning(PlayAreaSize.Row, PlayAreaSize.Column);
            // 縦列走査
            scaning(PlayAreaSize.Column, PlayAreaSize.Row);
            // TODO: 変身するお邪魔セル走査

            // 重複を取り除いて、row の降順 col の昇順に並び替え
            listC = listC.Distinct().OrderByDescending(c => c.row).ThenBy(c => c.col).ToList();

            // 消滅するセルを、消滅するように仕向ける
            for(int i=0; i<listC.Count; i++) {
                var cell = listC[i].Item3;
                // 状態遷移タイマーのセット
                cell.StateTimer = (flashTime, i * momentTime + 2, (listC.Count - i) * momentTime);
                // セルの状態をフラッシュに
                cell.Status = CellState.Flash;
            }

            // 全てのセルをアップデート
            for(int row = 0; row < CellArray.Row; row++) {
                for(int col=0; col<CellArray.Column; col++) {
                    CellArray[row, col].Update();
                }
            }


            //==============================お邪魔セルを落下させる？==============================
            // TODO: お邪魔セルを落下させる

        }

        /// <summary>
        ///   消えるセルを、対応したタプルにして返す
        /// </summary>
        private (int, int, CellInfo) GetEliminateCell(int row, int col)
                => (row, col, CellArray[row, col]);

        /// <summary>
        ///   <para>ゲームオーバー判定。_scrollWaitFrame が０の時にのみ呼ばれる</para>
        /// </summary>
        private bool IsGameOver()
        {
            // PlayAreaSize.Row-1 に、セルが一つでもあればゲームオーバーなので、
            // 一つ上に移動させる必要もない
            var row = PlayAreaSize.Row - 1;
            for(var col = 0; col < PlayAreaSize.Column; col++) {
                if(CellArray[row, col].CellType is not CellType.Empty)
                    return true;
            }
            return false;
        }
    }
}
