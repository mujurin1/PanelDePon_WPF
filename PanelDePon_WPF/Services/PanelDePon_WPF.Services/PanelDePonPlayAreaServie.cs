using PanelDePon.PlayArea;
using PanelDePon.Types;
using PanelDePon_WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon_WPF.Services
{
    public class PanelDePonPlayAreaServie : IPanelDePonPlayAreaService
    {
        private PlayArea _playArea;

        /// <summary>経過フレーム数</summary>
        public int ElapseFrame => _playArea.ElapseFrame;
        /// <summary>プレイエリアの広さ。row, col</summary>
        public AreaSize PlayAreaSize => _playArea.PlayAreaSize;
        /// <summary>
        ///   <para>プレイエリア内のセルの配列</para>
        ///   <para>注意）せり上がってくるセルは Row:-1</para>
        /// </summary>
        public RectangleArray<CellInfo> CellArray => _playArea.CellArray;
        /// <summary>せり上がっている今の高さ</summary>
        public double Border => _playArea.ScrollLine;
        /// <summary>カーソルの状態</summary>
        public CursorStatus CursorStatus => _playArea.CursorStatus;

        /// <summary>プレイエリアを１フレーム分更新する</summary>
        /// <param name="userOperation">ユーザーの操作</param>
        public void UpdateFrame(UserOperation userOperation) => _playArea.UpdateFrame(userOperation);

        /// <summary>プレイエリアの更新が全て終了した時に呼ばれる</summary>
        public event EventHandler Updated {
            add => _playArea.Updated += value;
            remove => _playArea.Updated -= value;
        }

        public PanelDePonPlayAreaServie()
        {
            this._playArea = new PlayArea(12, 6);
        }
    }
}
