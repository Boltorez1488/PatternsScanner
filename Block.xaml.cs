using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PatternsScanner {
    public partial class Block : INotifyPropertyChanged {
        public ObservableCollection<object> Items = new ObservableCollection<object>();
        public Block MainParent = null;

        private string _name;
        public string MainName {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public delegate void UpEvent(Block sender);
        public delegate void DownEvent(Block sender);
        public delegate void DeleteEvent(Block sender);

        public event UpEvent OnUp;
        public event DownEvent OnDown;
        public event DeleteEvent OnDelete;

        public Block(Block parent = null) {
            InitializeComponent();
            DataContext = this;
            items.ItemsSource = Items;
            MainName = "Block";
            MainParent = parent;
        }

        private void OnChildUp(object sender) {
            var index = Items.IndexOf(sender);
            if (index != 0) {
                Items.Move(index, index - 1);
            }
        }

        private void OnChildDown(object sender) {
            var index = Items.IndexOf(sender);
            if (index != Items.Count - 1) {
                Items.Move(index, index + 1);
            }
        }

        public void OnChildDelete(object sender) {
            var index = Items.IndexOf(sender);
            if (index >= 0) {
                switch (sender) {
                    case Block b:
                        b.OnUp -= OnChildUp;
                        b.OnDown -= OnChildDown;
                        b.OnDelete -= OnChildDelete;
                        break;
                    case Pattern p:
                        p.OnUp -= OnChildUp;
                        p.OnDown -= OnChildDown;
                        p.OnDelete -= OnChildDelete;
                        break;
                    case Field f:
                        f.OnUp -= OnChildUp;
                        f.OnDown -= OnChildDown;
                        f.OnDelete -= OnChildDelete;
                        break;
                }
            }
            Items.RemoveAt(index);
            if (index != 0 && index == Items.Count)
                index--;
            if (Items.Count == 0) {
                dummy.Visibility = Visibility.Visible;
                scroll.Focus();
            } else if (Items[index] is Block b) {
                b.scroll.Focus();
            } else if (Items[index] is Pattern p) {
                p.comment.Focus();
            } else if (Items[index] is Field f) {
                f.comment.Focus();
            }
        }

        public void ParentReplace(Block parent = null) {
            MainParent = parent;
        }

        public Block AddBlock() {
            if (!expander.IsExpanded)
                expander.IsExpanded = true;
            var child = new Block(this);
            child.OnUp += OnChildUp;
            child.OnDown += OnChildDown;
            child.OnDelete += OnChildDelete;
            Items.Add(child);
            dummy.Visibility = Visibility.Collapsed;
            return child;
        }

        public Pattern AddPattern() {
            if (!expander.IsExpanded)
                expander.IsExpanded = true;
            var child = new Pattern {MainParent = this};
            child.OnUp += OnChildUp;
            child.OnDown += OnChildDown;
            child.OnDelete += OnChildDelete;
            Items.Add(child);
            dummy.Visibility = Visibility.Collapsed;
            return child;
        }

        public Field AddField(string left = null, string comment = null) {
            if (!expander.IsExpanded)
                expander.IsExpanded = true;
            var child = left == null && comment == null ? 
                new Field {MainParent = this} : 
                new Field(left, comment) { MainParent = this };
            child.OnUp += OnChildUp;
            child.OnDown += OnChildDown;
            child.OnDelete += OnChildDelete;
            Items.Add(child);
            dummy.Visibility = Visibility.Collapsed;
            return child;
        }

        private void Up() {
            OnUp?.Invoke(this);
        }

        private void Down() {
            OnDown?.Invoke(this);
        }

        public void Delete() {
            OnDelete?.Invoke(this);
        }

        public void FocusName() {
            nameBox.Focus();
            nameBox.SelectAll();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            FocusName();
        }

        private void nameBox_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                e.Handled = true;
                Keyboard.ClearFocus();
                scroll.Focus();
            }
        }

        private void delBtn_Click(object sender, RoutedEventArgs e) {
            Delete();
        }

        private void addBBtn_Click(object sender, RoutedEventArgs e) {
            AddBlock();
        }

        private void addPBtn_Click(object sender, RoutedEventArgs e) {
            AddPattern();
        }

        private void upBtn_Click(object sender, RoutedEventArgs e) {
            Up();
        }

        private void downBtn_Click(object sender, RoutedEventArgs e) {
            Down();
        }

        private void FocusName(object sender, ExecutedRoutedEventArgs e) {
            FocusName();
        }

        private void DeleteCurrent(object sender, ExecutedRoutedEventArgs e) {
            Delete();
        }

        private void BlockAdder(object sender, ExecutedRoutedEventArgs e) {
            AddBlock();
        }

        private void PatternAdder(object sender, ExecutedRoutedEventArgs e) {
            AddPattern();
        }

        private void Up(object sender, ExecutedRoutedEventArgs e) {
            Up();
        }

        private void Down(object sender, ExecutedRoutedEventArgs e) {
            Down();
        }

        private void addFBtn_Click(object sender, RoutedEventArgs e) {
            AddField();
        }

        private void FieldAdder(object sender, ExecutedRoutedEventArgs e) {
            AddField();
        }

        public void AttachBlock(Block child) {
            var st = AddBlock();
            st.MainName = child.MainName;
            foreach (var item in child.Items) {
                switch (item) {
                    case Block obj:
                        obj.OnDelete -= child.OnChildDelete;

                        obj.OnDelete += st.OnChildDelete;
                        obj.ParentReplace(st);
                        break;
                }
                st.Items.Add(item);
            }
            if (st.Items.Count != 0) {
                st.dummy.Visibility = Visibility.Collapsed;
            }
        }

        public void AttachField(Field child) {
            AddField(child.Left, child.Comment);
        }

        public void AttachPattern(Pattern child) {
            var p = AddPattern();
            p.CheckByte = child.CheckByte;
            p.Comment = child.Comment;
            p.NumberExtractor = child.NumberExtractor;
            p.Offset = child.Offset;
            p.PatternBytes = child.PatternBytes;
        }

        private void MoveBtn_OnClick(object sender, RoutedEventArgs e) {
            var dlg = new ElementMover.MainDialog(this, MainParent);
            if (dlg.ShowDialog() == true) {
                if (dlg.Selected == null) {
                    OnDelete?.Invoke(this);
                    Constants.MainWindow.AttachBlock(this);
                    return;
                }

                var b = dlg.Selected;
                OnDelete?.Invoke(this);
                b.AttachBlock(this);
            }
        }
    }
}