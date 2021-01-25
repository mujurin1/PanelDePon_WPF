using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon.Types
{
    /// <summary>
    ///   そのマスのセルの情報
    /// </summary>
    public struct CellInfo
    {
        /// <summary>空のCellInfoを返す</summary>
        public static readonly CellInfo Empty = new CellInfo();

        /// <summary>
        ///   <para>CellType列挙型の、普通のセルの数</para>
        ///   <para>CellType列挙型に強く依存するので、CellType列挙型の変更に注意する</para>
        /// </summary>
        private readonly static int NormalCellTypes =
                    Enum.GetValues<CellType>()
                        .Select(e => (int)e)
                        .Where(n => (n & 0x10) == 0x10).Count();

        private (int Flash, int Neutral, int Lock) _stateTimer;
        /// <summary>
        ///   <para>セルの状態遷移に使うタイマー</para>
        ///   <para>Lock にセットした時間は TransitionTime にも入る</para>
        /// </summary>
        public (int Flash, int Neutral, int Lock) StateTimer {
            get => _stateTimer;
            set {
                _stateTimer = value;
                TransitionTime = value.Lock;
            }
        }
        /// <summary>
        ///   移動に掛かる時間
        /// </summary>
        public double TransitionTime { get; private set; }

        /// <summary>セルの種類</summary>
        public CellType CellType;
        /// <summary>セルの状態。自由、落下中、消滅.変身.待機中</summary>
        public CellState Status;
        /// <summary>
        ///   <span>この値が１以上なら、State は CellType.Wait</span>
        ///   <span>この値が０になった次のフレームから落下し始める（或いはFreeになる）</span>
        /// </summary>
        public int WaitFrame;
        /// <summary>お邪魔セルが、同じお邪魔か別のお邪魔か判別するためのID</summary>
        public uint ID;

        /// <summary>
        ///   <para>そのセルを、削除に使えるかどうか</para>
        ///   <para>状態：Free　種類：ノーマルセル</para>
        /// </summary>
        public bool IsEliminatable
            => CellType.IsNomal() && Status is CellState.Free;


        /// <summary>
        ///   セルは、一度生成されたら、それをずっと使い続ける
        /// </summary>
        private CellInfo(CellType type)
        {
            this.ID = 0;
            this._stateTimer = (0, 0, 0);
            this.TransitionTime = 0;
            this.CellType = type;
            this.Status = CellState.Free;
            this.WaitFrame = 0;
        }

        // アップデートは２種類
        // 入れ替わった   セル自体の更新はしない
        // それ以外       セル自体の更新

        /// <summary>
        ///   １フレーム経過
        /// </summary>
        public CellInfo Update()
        {
            if(Status is CellState.Free && CellType is CellType.Empty) return this;
            /* [下のSwitch文について]
             * セルの状態を時間経過で遷移させるためのもの
             * _timer に入っている数値の意味は
             *      状態：フラッシュをいつまで続けるか
             *      状態：ニュートラルをいつまで続けるか
             *      状態：ロックをいつまで続けるか
             * セルの状態は、
             * ０フリー → １フラッシュ → 2.1ニュートラル → 2.2モーメント → ３ロック →　０フリー
             * の順で遷移するため、必ず上から順に、数字の順番に実行する
             * 
             * また、タイマーには、３つの値しか無いが、
             * モーメントの状態は１フレームだけなので、
             *      (2.1)ニュートラルが実行される
             *      _timer.Neutral に 1 を入れる
             *      状態をモーメントにする
             *      １フレーム後に、(2.3)モーメントが実行される
             *      状態をロックにする
             *      セルの種類によってセルの種類を変更する
             *      (３)ロックがNフレーム後実行される
             * という風に処理して、ニュートラルタイマーを２回利用してる
             */
            // 数値１～５は実行順序
            switch(StateTimer) {
            case ( > 0, > 0, > 0):              // １　　Status：Flash
                if(--_stateTimer.Flash == 0)
                    Status = CellState.Neutral;
                break;

            case (0, > 0, > 0):                 // ２　　Status：Neutral or Moment
                if(--_stateTimer.Neutral == 0) {
                    if(Status is CellState.Neutral) {   // 2.1　Status：Neutral
                        Status = CellState.Moment;
                        _stateTimer.Neutral = 1;             // 　　NeutralTimerを再セット。次はケース２→分岐で４に入る
                    } else {                            // 2.2　Status：Moment
                        Status = CellState.Lock;
                        if(CellType.IsNomal()) {            // 通常orびっくりセル
                            CellType = CellType.Empty;
                        } else {                            // お邪魔セル
                            CellType = RandomCellType(n: -1);
                        }
                    }
                }
                break;

            case (0, 0, > 0):                   // ３　State：Lock
                if(--_stateTimer.Lock == 0)
                    Status = CellState.Free;
                break;
            }
            return this;
        }

        private static readonly Random _random = new Random();
        /* 生成されるセル
         * 普通のセルの条件：5bit目が1
         * CellType 0x10 ~ 0x14 (16 ~ 20)
         */
        /// <summary>
        ///   <para>ランダムな普通のセルタイプを返却する</para>
        /// </summary>
        /// <param name="n">びっくりセルを含める:0  含めない:-1</param>
        public static CellType RandomCellType(int n)
            => (CellType)_random.Next(0x10, 0x10 + NormalCellTypes + n);
    }
}