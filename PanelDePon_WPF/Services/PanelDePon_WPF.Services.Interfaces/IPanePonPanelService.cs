using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon_WPF.Services.Interfaces
{
    /// <summary>
    ///   <para>パネポンのプレイエリアインターフェース</para>
    ///   <pane>セルがあるパネルと、セルのインターフェース</pane>
    /// </summary>
    public interface IPanePonPanelService
    {
        /// <summary>セルの横列数</summary>
        public int CellColumns { get; set; }
        /// <summary>セルの縦列数</summary>
        public int CellRows { get; set; }
        /// <summary>表示倍率</summary>
        public int Scale { get; set; }
        /// <summary>パネポンパネルのデザイン。まだないので予約</summary>
        public object PanelDesign { get; set; }
        /// <summary>パズルセルのデザイン。まだないので予約</summary>
        public object PazzleCellDesign { get; set; }
        /// <summary>お邪魔セルのデザイン。まだないので予約</summary>
        public object OjamaCellDesign { get; set; }
    }
}
