using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PatternsScanner {
    class PatternValidator : ValidationRule {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            string strValue = Convert.ToString(value);

            if (string.IsNullOrEmpty(strValue))
                return new ValidationResult(false, "Value cannot be coverted to string.");

            if (Utils.IsMatchRegex(strValue, new Regex(@"((([0-9A-F]{2})|(\?))[ ]{0,1})+")))
                return new ValidationResult(true, null);
            return new ValidationResult(false, "Pattern is bad");
        }
    }
}
