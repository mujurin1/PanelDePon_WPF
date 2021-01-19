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
        /// <summary>
        ///   表示倍率
        /// </summary>
        public double Scare {
            get => PonPanel.Scale;
            set => PonPanel.Scale = value;
        }
        public override int BaseWidth => 180;
        public override int BaseHeight => 360;

        public PonPanel() : base()
        {
            InitializeComponent();

            var cell = new PazzleCell(1, 1);
            PanePonPanel.Children.Add(cell);
            Task.Run(async () => {
                var wait = 1000;
                await Task.Delay(wait);
                Dispatcher.Invoke(() => PanePonControlAbs.Scale *= 1.5);
                await Task.Delay(wait);
                Dispatcher.Invoke(() => cell.CanvasLeft += 20);
            });
        }

        protected override void ChangeScale(double old)
        {
            Width = BaseWidth * Scare;
            Height = BaseHeight * Scare;
            
        }
    }
}
