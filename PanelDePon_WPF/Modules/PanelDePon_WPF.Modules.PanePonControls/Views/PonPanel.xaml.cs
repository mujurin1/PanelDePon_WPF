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
    /// PonPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class PonPanel : UserControl
    {
        private int _viewRatio = 1;
        /// <summary>
        ///   表示倍率
        /// </summary>
        public int ViewRatio {
            get => _viewRatio;
            set {
                _viewRatio = value;
                Width = BaseWidth * ViewRatio;
                Height = BaseHeight * ViewRatio;
            }
        }
        public int BaseWidth => 30;
        public int BaseHeight => 30;

        public PonPanel()
        {
            InitializeComponent();

            var square = new SquareCell(30, 30);
            PanePonPanel.Children.Add(square);
            Debug.WriteLine($"Left {Canvas.GetLeft(square)}  Top { Canvas.GetTop(square)}");

            Task.Run(async () => {
                var wait = 1000;
                await Task.Delay(wait);
                Dispatcher.Invoke(() => square.CanvasLeft += 30);
            });
        }
    }
}
