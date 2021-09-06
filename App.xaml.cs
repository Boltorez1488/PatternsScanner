using System.IO;
using System.Windows;

namespace PatternsScanner {
    public partial class App {
        private void Application_Startup(object sender, StartupEventArgs e) {
            if (e.Args.Length == 1 && File.Exists(e.Args[0])) {
                var wnd = new PatternsBrowser(false);
                wnd.Open(e.Args[0]);
            } else {
                var browser = new PatternsBrowser();
            }
        }
    }
}