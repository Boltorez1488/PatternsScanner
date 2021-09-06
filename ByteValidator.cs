using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PatternsScanner {
    class ByteValidator : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            string strValue = Convert.ToString(value);

            if (Utils.IsMatchRegex(strValue, new Regex(@"[A-Fa-f0-9]{0,1}")))
                return new ValidationResult(true, null);
            return new ValidationResult(false, "Byte is bad");
        }
    }
}
