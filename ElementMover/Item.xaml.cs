using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PatternsScanner.ElementMover {
    public partial class Item : INotifyPropertyChanged {
        public delegate void SelectedEvent(Item sender);
        public event SelectedEvent OnSelect;
        public Block Root;

        private Color _prefixColor;
        public Color PrefixColor {
            get => _prefixColor;
            set {
                _prefixColor = value;
                OnPropertyChanged();
            }
        }

        public string PtrPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void AddText(string text, object color) {
            TextRange select = new TextRange(nameBlock.ContentEnd, nameBlock.ContentEnd) { Text = text };
            select.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        private List<Block> GetParts() {
            List<Block> parts = new List<Block>();
            if (Root is Block root) {
                parts.Add(root);
                Block parent = root.MainParent;
                while (parent != null) {
                    parts.Add(parent);
                    parent = parent.MainParent;
                }
            }
            parts.Reverse();
            return parts;
        }

        private void BuildPath() {
            var parts = GetParts();
            var last = parts.Last();
            foreach (var p in parts) {
                AddText(p.MainName, new SolidColorBrush(Colors.DarkTurquoise));
                if (p != last)
                    AddText(".", Brushes.White);
            }
        }

        private void SetName(string parent, string name, string del = ".") {
            nameBlock.Text = "";
            if (parent == null) {
                PtrPath = name;
                BuildPath();
            } else {
                PtrPath = parent + del + name;
                BuildPath();
            }
        }

        public Item(Block root) {
            InitializeComponent();
            DataContext = this;

            Root = root;
            //if (root.ParentPath is string parent) {
            //    SetName(root.MainParent != null && parent.Length != 0 ? parent : null, root.MainName);
            //}
            SetName(null, root.MainName);

            prefix.Text = "BLOCK";
            PrefixColor = Colors.DarkTurquoise;
        }

        public Item() {
            InitializeComponent();
            DataContext = this;

            Root = null;
            nameBlock.Text = "";
            TextRange mainName = new TextRange(nameBlock.ContentEnd, nameBlock.ContentEnd) { Text = "Root" };
            mainName.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.DarkOrange));

            prefix.Text = "WINDOW";
            PrefixColor = Colors.DarkOrange;
        }

        private void grid_MouseDown(object sender, MouseButtonEventArgs e) {
            OnSelect?.Invoke(this);
        }
    }
}