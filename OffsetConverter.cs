using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PatternsScanner {
    public class OffsetConverter : IValueConverter {
        // To HexString
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null)
                return "0";

            var intVal = (int)value;
            if (intVal < 0) {
                intVal *= -1;
                return $"-{intVal:X}";
            }

            return $"{intVal:X}";
        }

        // To Int
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            var strVal = value?.ToString();
            if (string.IsNullOrEmpty(strVal))
                return 0;

            if (strVal[0] == '-') {
                return int.Parse(strVal.Substring(1), NumberStyles.HexNumber) * -1;
            }
            return int.Parse(strVal, NumberStyles.HexNumber);
        }
    }
}
