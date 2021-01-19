using PanelDePon_WPF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    /// <summary>
    ///   <pane>パネポンのパズルを表示するエリア</pane>
    /// </summary>
    public partial class PonPanel : PanePonControlAbs
    {
        private int _columns = 6;
        /// <summary>
        ///   横列のパズルセルの数
        /// </summary>
        public int Columns {
            get => _columns;
            set {
                _columns = value;
                Width = CellSize * Columns;
            }
        }
        private int _rows = 12;
        /// <summary>
        ///   縦列のパズルセルの数
        /// </summary>
        public int Rows {
            get => _rows;
            set {
                _rows = value;
                Height = CellSize * Rows;
            }
        }

        public PonPanel() : base()
        {
            InitializeComponent();
        }
    }
}
