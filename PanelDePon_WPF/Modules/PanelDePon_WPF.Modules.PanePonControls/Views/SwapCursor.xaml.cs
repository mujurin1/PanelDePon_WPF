using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Matrix = PanelDePon.Types.Matrix;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using PanelDePon_WPF.Services.Interfaces;

namespace PanelDePon_WPF.Modules.PanePonControls.Views
{
    /// <summary>
    /// SwapCursor.xaml の相互作用ロジック
    /// </summary>
    public partial class SwapCursor : PlayAreaControlAbs
    {
        public SwapCursor(IPanelDePonPlayAreaService playAreaService, Matrix matrix) : base(playAreaService, matrix)
        {
            InitializeComponent();
            this.BorderBrush = Brushes.Pink;
            this.BorderThickness = new Thickness(1);

            Width = CellSize << 1;
            Height = CellSize;
        }

        public void Update()
        {
            if(!Matrix.Equals(_playAreaService.CursorStatus.CursorPos))
                Move(_playAreaService.CursorStatus.CursorPos);
        }
    }
}
