using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PatternsScanner {
    class Logger {
        public static TextBox LogBox = null;

        public static void Log(string text) {
            if (LogBox == null)
                return;
            var dt = DateTime.Now;
            LogBox.AppendText($"[{dt:MM.dd.yy H:mm:ss}]: {text}\n");
            LogBox.ScrollToEnd();
        }
    }
}
