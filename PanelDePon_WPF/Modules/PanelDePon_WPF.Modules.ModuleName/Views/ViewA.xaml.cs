using PanelDePon_WPF.Modules.PanePonControls.Views;
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

namespace PanelDePon_WPF.Modules.ModuleName.Views
{
    /// <summary>
    /// Interaction logic for ViewA.xaml
    /// </summary>
    public partial class ViewA : UserControl
    {
        public ViewA()
        {
            // 不要説 http://ex.osaka-kyoiku.ac.jp/~fujii/WPF/Anime.html
            InitializeComponent();

            var square = new SquareCell(40, 40);
            //Canvas.SetLeft(square, 40);
            //Canvas.SetTop(square, 40);

            GameArea.Children.Add(square);
            Task.Run(async () => {
                int wait = 200;
                await Task.Delay(wait);
                this.Dispatcher.Invoke(() => square.CanvasLeft += 20);
                await Task.Delay(wait);
                this.Dispatcher.Invoke(() => square.CanvasLeft += 20);
                await Task.Delay(wait);
                this.Dispatcher.Invoke(() => square.CanvasTop -= 20);
                await Task.Delay(wait);
                this.Dispatcher.Invoke(() => square.CanvasTop -= 20);
            });
        }
    }
}
