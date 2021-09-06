using System;
using System.Collections.Generic;
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

namespace PatternsScanner {
    public partial class PatternItem {
        public PatternItem(string fpath) {
            InitializeComponent();

            var name = System.IO.Path.GetFileNameWithoutExtension(fpath);
            fname.Text = name;
            path.Text = fpath;
        }

        public delegate void SelectEvent(PatternItem sender);

        public event SelectEvent OnSelected;

        private void Grid_OnMouseDown(object sender, MouseButtonEventArgs e) {
            OnSelected?.Invoke(this);
        }
    }
}
