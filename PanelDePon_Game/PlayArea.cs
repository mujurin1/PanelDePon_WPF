using PanelDePon_Game.Enums;
using PanelDePon_Game.Lib;
using System;
using System.Collections.ObjectModel;

namespace PanelDePon_Game
{
    public class PlayArea
    {
        /// <summary>
        ///   <para>セルの情報を格納した配列</para>
        ///   <para>一番下の列は、せり上がってるセル</para>
        ///   <para>左下が row:0 col:0 で右上に行くほど増加</para>
        /// </summary>
        private RectangleArray<CellInfo> _cellArray;
        /// <summary>_cellArrayの公開用。値型のプロパティなので、変更は不可能</summary>
        public RectangleArray<CellInfo> CellArray => _cellArray;

        /// <summary>経過フレーム数</summary>
        public int ElapseFrame { get; set; } = 0;
        /// <summary>プレイエリアのサイズ</summary>
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
        /// <summary>スクロール待機フレーム数。０ならスクロールする</summary>
        private uint _scrollWaitFrame = 0;
        /// <summary>スクロール上がる速度</summary>
        public double UpSpeed { get; private set; }
        /// <summary>せり上がっている今の高さ</summary>
        public double Border{ get; private set; }
        /// <summary>Border がこの値まで来たら、完全にセルが上に移動する</summary>
        private static double BorderLine => 10;

        /// <summary>
        ///   ユーザーの入力と、カーソルの状態
        /// </summary>
        public CursorStatus CursorStatus { get; private set; }

        public PlayArea(int row, int col)
        {
            this.PlayAreaSize = new AreaSize(row, col);
            // セルの存在する２次元配列
            // rows（縦列）はお邪魔が降って来て画面上範囲外にセルが存在するので、1.5倍にして、
            // 　　　　　　　一番下はせり上がる列なので更に＋１
            this._cellArray = new RectangleArray<CellInfo>((int)(row*1.5) + 1, col);
            this.CursorStatus = new CursorStatus(PlayAreaSize);
            StartGame();
        }

        /// <summary>
        ///   <para>ゲームエリアの状態を初期化する</para>
        /// </summary>
        public void StartGame()
        {
            // セルの配置を初期化する
            _cellArray = new RectangleArray<CellInfo>(_cellArray.Row, _cellArray.Column);
            for(int row = -1; row <= 5; row++) {
                for(int col = 0; col <= 5; col++) {
                    _cellArray[row, col] = CellInfo.Random();
                }
            }
        }

        /// <summary>
        ///   プレイエリアを１フレーム分更新する
        /// </summary>
        /// <param name="cursorOperation">カーソルの操作</param>
        public void UpdateFrame(UserOperation cursorOperation)
        {
            GameRule(cursorOperation);
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
            //---------ユーザーの操作
            switch(userOperation) {
            case UserOperation.NaN:
                break;
            case >= UserOperation.CursorUp and <= UserOperation.ClickChangeCell:
                // カーソル移動系 or クリック操作なら、カーソルに処理を渡して、ごーとぅー
                CursorStatus.UpdateFrame(userOperation);
                goto CursorUpdateCompleted;
            case UserOperation.ChangeCell:
                // TODO: セルを入れ替える
                break;
            case UserOperation.ScrollSpeedUp:
                // TODO: スクロール速度が上昇する
                break;
            }
            // カーソルの移動系以外の操作だった場合に来る
            // カーソルの状態を更新する
            CursorStatus.UpdateFrame();

            CursorUpdateCompleted:


            //---------スクロールする
            // スクロール待機中なら、待機フレーム数を１減らしてスクロールしない
            if(_scrollWaitFrame > 0) {
                _scrollWaitFrame--;
            } else {
                // TODO: セルをせり上げる
                // 注意）スクロール速度アップの場合もある
                // 更に、ここでゲームオーバー処理へと続く..
            }

            
            //---------セルの状態を更新する
            // 左下(0,0) から、右上(n,n)に向かって
            // TODO: セルの状態を更新する


            //---------お邪魔セルを落下させる？
            // TODO: お邪魔セルを落下させる
        }
    }

}
