using PanelDePon_WPF.Modules.PanePonControls.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PanelDePon_WPF.Modules.PanePonControls.Converters
{
    /// <summary>
    ///   つかってない!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    class CellMoveConverter : IValueConverter
    {
        /// <summary>
        ///   この値に基づいてトリガーを起動する
        /// </summary>
        private SquareCell _squareCell;
        /// <summary>
        ///   この値が条件を満たすかを調べる
        /// </summary>
        private Func<double> _inspection;

        public CellMoveConverter(SquareCell squareCell, Func<double> inspection)
        {
            this._squareCell = squareCell;
            this._inspection = inspection;
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var v = System.Convert.ToDouble(value);  検査
            var comparaValue = double.Parse(parameter as string);
            return _inspection() != comparaValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
