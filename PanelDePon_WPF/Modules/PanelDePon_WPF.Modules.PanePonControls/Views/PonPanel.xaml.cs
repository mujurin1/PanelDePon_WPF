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
    public partial class PonPanel : UserControl
    {
        private int _scare = 1;
        /// <summary>
        ///   表示倍率
        /// </summary>
        public int Scare {
            get => _scare;
            set {
                _scare = value;
                Width = BaseWidth * Scare;
                Height = BaseHeight * Scare;
            }
        }
        public int BaseWidth => 180;
        public int BaseHeight => 360;

        public PonPanel()
        {
            InitializeComponent();
            this.Scare = 1;
        }
    }
}
