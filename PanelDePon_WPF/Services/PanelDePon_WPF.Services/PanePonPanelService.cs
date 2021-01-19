using PanelDePon_WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanelDePon_WPF.Services
{
    public class PanePonPanelService : IPanePonPanelService
    {
        /// <summary>セルの横列数</summary>
        public int CellColumns { get; set; }
        /// <summary>セルの縦列数</summary>
        public int CellRows { get; set; }
        /// <summary>パネポンパネルのデザイン。まだないので予約</summary>
        public object PanelDesign { get; set; }
        /// <summary>パズルセルのデザイン。まだないので予約</summary>
        public object PazzleCellDesign { get; set; }
        /// <summary>お邪魔セルのデザイン。まだないので予約</summary>
        public object OjamaCellDesign { get; set; }

        public PanePonPanelService(int col, int row)
        {
            this.CellColumns = col;
            this.CellRows = row;
        }
    }
}
