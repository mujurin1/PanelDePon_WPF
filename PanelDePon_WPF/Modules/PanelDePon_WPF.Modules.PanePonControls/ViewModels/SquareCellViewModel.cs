﻿using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PanelDePon_WPF.Modules.PanePonControls.ViewModels
{
    public class SquareCellViewModel : BindableBase
    {
        public Rectangle Rect { get; set; }

        public ReactivePropertySlim<double> Size { get; set; } = new(20);

        public SquareCellViewModel()
        {
        }
    }
}
