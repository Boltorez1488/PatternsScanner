using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
//using PatternsScanner.PathSelector;

namespace PatternsScanner.ElementMover {
    public partial class MainDialog {
        public ObservableCollection<Item> Items = new ObservableCollection<Item>();
        public ObservableCollection<Item> SearchItems = new ObservableCollection<Item>();
        public Block Selected;
        public object Caller;
        public object CallerParent;

        public MainDialog(object caller, object parent) {
            InitializeComponent();
            DataContext = this;
            Caller = caller;
            CallerParent = parent;
            items.ItemsSource = Items;

            // MainWindow
            if (parent != null && caller is Block) {
                var item = new Item();
                item.OnSelect += OnSelect;
                Items.Add(item);
            }

            Fill();
        }

        private void OnSelect(Item sender) {
            Selected = sender.Root;
            DialogResult = true;
            Close();
        }

        private bool IsAllow(object obj) {
            switch (obj) {
                case Block b:
                    if (CallerParent != null && Equals(b, CallerParent))
                        return false;
                    if (Equals(b, Caller))
                        return false;
                    break;
            }
            return true;
        }

        private void Block(Block root) {
            foreach (var obj in root.Items) {
                if (obj is Block space) {
                    if (IsAllow(obj)) {
                        var item = new Item(space);
                        item.OnSelect += OnSelect;
                        Items.Add(item);
                    }
                    if (!Equals(Caller, space))
                        Block(space);
                }
            }
        }

        private void Fill() {
            foreach (var obj in Constants.MainWindow.Items) {
                if (obj is Block b) {
                    if (IsAllow(obj)) {
                        var item = new Item(b);
                        item.OnSelect += OnSelect;
                        Items.Add(item);
                    }
                    if (!Equals(Caller, b))
                        Block(b);
                }
            }
        }

        private void SearchBox_OnKeyUp(object sender, KeyEventArgs e) {
            if (String.IsNullOrEmpty(searchBox.Text)) {
                items.ItemsSource = Items;
            } else {
                var search = searchBox.Text;
                var list = Items.Where(x => x.PtrPath.StartsWith(search));
                SearchItems.Clear();
                foreach (var item in list)
                    SearchItems.Add(item);
                items.ItemsSource = SearchItems;
            }
        }
    }
}