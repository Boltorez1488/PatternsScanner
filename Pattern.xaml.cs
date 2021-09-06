using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class Pattern : INotifyPropertyChanged {
        public Block MainParent = null;
        public List<int> LastSearch;
        public bool IsSearchSuccess = false;

        public string GetPath() {
            var name = string.IsNullOrEmpty(Comment) ? "Pattern" : Comment;
            if (MainParent == null) {
                return $"[{name}]";
            }
            name = $"[{name}]";

            var parent = MainParent;
            do {
                name = $"[{parent.MainName}]{name}";
                parent = parent.MainParent;
            } while (parent != null);

            return name;
        }

        private int _offset;
        public int Offset {
            get => _offset;
            set {
                _offset = value;
                OnPropertyChanged();
            }
        }

        private string _checkByte;
        public string CheckByte {
            get => _checkByte;
            set {
                _checkByte = value;
                OnPropertyChanged();
            }
        }

        private int _numberExctractor;
        public int NumberExtractor {
            get => _numberExctractor;
            set {
                _numberExctractor = value;
                if (_numberExctractor < 0)
                    _numberExctractor = 0;
                OnPropertyChanged();
            }
        }

        private string _patternBytes;
        public string PatternBytes {
            get => _patternBytes;
            set {
                _patternBytes = value;
                OnPropertyChanged();
            }
        }

        private string _comment;
        public string Comment {
            get => _comment;
            set {
                _comment = value;
                OnPropertyChanged();
            }
        }

        public struct Builded {
            public string Mask;
            public byte[] Bytes;

            public Builded(byte[] bytes, string mask) {
                Bytes = bytes;
                Mask = mask;
            }
        }
        public Builded Build() {
            var mask = "";
            var split = PatternBytes.Trim().Split(' ');
            var list = new List<byte>();
            foreach (var s in split)
                if (s != "?") {
                    list.Add(byte.Parse(s, NumberStyles.HexNumber));
                    mask += "x";
                } else {
                    list.Add(0x0);
                    mask += "?";
                }

            return new Builded(list.ToArray(), mask);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public delegate void UpEvent(Pattern sender);
        public delegate void DownEvent(Pattern sender);
        public delegate void DeleteEvent(Pattern sender);

        public event UpEvent OnUp;
        public event DownEvent OnDown;
        public event DeleteEvent OnDelete;

        public Pattern() {
            InitializeComponent();
            DataContext = this;

            Offset = 0x0;
            Comment = "Description";
            PatternBytes = "00";
            CheckByte = "";
            NumberExtractor = 0;
        }

        private void Up() {
            OnUp?.Invoke(this);
        }

        private void Down() {
            OnDown?.Invoke(this);
        }

        private void Delete() {
            OnDelete?.Invoke(this);
        }

        private void CheckByte_OnMouseWheel(object sender, MouseWheelEventArgs e) {
            var parsed = string.IsNullOrEmpty(CheckByte) ? 0 : int.Parse(CheckByte, NumberStyles.HexNumber);
            var val = parsed + Utils.CalcWheel(e.Delta);
            if (val > 0xF) {
                val = 0x0;
            } else if (val < 0) {
                val = 0xF;
            }
            CheckByte = $"{val:X}";
        }

        private void Offset_OnMouseWheel(object sender, MouseWheelEventArgs e) {
            /*if (Offset[0] == '-') {
                var val = int.Parse(Offset.Substring(1), NumberStyles.HexNumber) - Utils.CalcWheel(e.Delta);
                if (val > 0) {
                    Offset = $"-{val:X}";
                } else {
                    val = -1 * val;
                    Offset = $"{val:X}";
                }
            } else {
                var val = int.Parse(Offset, NumberStyles.HexNumber) + Utils.CalcWheel(e.Delta);
                if (val < 0) {
                    val = -1 * val;
                    Offset = $"-{val:X}";
                } else {
                    Offset = $"{val:X}";
                }
            }*/
            Offset += Utils.CalcWheel(e.Delta);
        }

        private void delBtn_Click(object sender, RoutedEventArgs e) {
            Delete();
        }

        private void upBtn_Click(object sender, RoutedEventArgs e) {
            Up();
        }

        private void downBtn_Click(object sender, RoutedEventArgs e) {
            Down();
        }

        private void Pattern_OnLoaded(object sender, RoutedEventArgs e) {
            pattern.Focus();
            pattern.SelectAll();
        }

        private void DeleteCurrent(object sender, ExecutedRoutedEventArgs e) {
            Delete();
        }

        private void Up(object sender, ExecutedRoutedEventArgs e) {
            Up();
        }

        private void Down(object sender, ExecutedRoutedEventArgs e) {
            Down();
        }

        private void NumEx_OnMouseWheel(object sender, MouseWheelEventArgs e) {
            NumberExtractor += Utils.CalcWheel(e.Delta);
        }

        private void MoveBtn_OnClick(object sender, RoutedEventArgs e) {
            var dlg = new ElementMover.MainDialog(this, MainParent);
            if (dlg.ShowDialog() == true) {
                if (dlg.Selected != null) {
                    var b = dlg.Selected;
                    OnDelete?.Invoke(this);
                    b.AttachPattern(this);
                }
            }
        }
    }
}
