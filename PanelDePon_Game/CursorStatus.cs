using PanelDePon_Game.Enums;
using PanelDePon_Game.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon_Game
{
    /// <summary>
    ///   ユーザーが操作するカーソルの状態
    /// </summary>
    public record CursorStatus
    {
        /// <summary>プレイエリアサイズ</summary>
        public AreaSize PlayAreaSize;
        /// <summary>
        ///   <para>カーソルの位置（左側）</para>
        ///   <para>左下が row:0 col:0 で右上に行くほど増加</para>
        /// </summary>
        public AreaSizeRange CursorPos { get; private set; }
        /// <summary>
        ///   <para>この値が０なら、カーソルの操作や入れ替えができる</para>
        ///   <para>０でないなら１フレーム毎にマイナス１</para>
        /// </summary>
        public uint CursorWait;

        public CursorStatus(AreaSize playAreaSize)
        {
            this.PlayAreaSize = playAreaSize;
            // カーソルは、プレイエリアの横列-1の範囲にしか存在しないため
            playAreaSize.Column--;
            this.CursorPos = new AreaSizeRange(playAreaSize);
        }

        /// <summary>
        ///   <para>カーソルの状態を１フレーム更新する</para>
        ///   <para>userOperation は、カーソル操作系しかあり得ない</para>
        /// </summary>
        /// <param name="userOperation">ユーザーの操作</param>
        public void UpdateFrame(UserOperation userOperation)
        {
            // カーソルが操作不能なら
            if(CursorWait > 0) {
                CursorWait--;
                return;
            }
            switch(userOperation) {
            case UserOperation.CursorUp:
                CursorPos.Row++;
                break;
            case UserOperation.CursorLeft:
                CursorPos.Column--;
                break;
            case UserOperation.CursorRight:
                CursorPos.Column++;
                break;
            case UserOperation.CursorDown:
                CursorPos.Row--;
                break;
            case UserOperation.ClickChangeCell:
                throw new NotImplementedException("ユーザーの操作例外。クリックしてセルを入れ替える処理はまだ実装していません。がんばって");
            default:
                throw new ArgumentException($"ユーザーの操作系以外の値が渡されました。入力された値：{userOperation}",
                                            "userOperation");
            }
        }

        /// <summary>
        ///   カーソルの状態を１フレーム更新する
        /// </summary>
        public void UpdateFrame()
        {
            if(CursorWait > 0) CursorWait--;
        }
    }
}
