using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;

namespace PatternsScanner {
    public partial class PatternsBrowser {
        public ObservableCollection<PatternItem> Items = new ObservableCollection<PatternItem>();
        public MainWindow OpenedPattern;

        public PatternsBrowser(bool needOpen = true) {
            InitializeComponent();
            items.ItemsSource = Items;
            var open = Load();
            if (!needOpen) 
                return;
            if (open != null && File.Exists(open)) {
                Open(open);
            } else {
                Show();
            }
        }

        private void OnSelect(PatternItem sender) {
            if (!File.Exists(sender.path.Text)) {
                Items.Remove(sender);
                return;
            }
            var win = new MainWindow(sender.path.Text, this);
            win.Show();
            Hide();
        }

        public void Open(string fpath) {
            AddPath(fpath);
            var win = new MainWindow(fpath, this);
            win.Show();
            Hide();
        }

        private void AddPath(string fpath) {
            var count = Items.Count(x => x.path.Text == fpath);
            if (count != 0) {
                var found = Items.First(x => x.path.Text == fpath);
                var index = Items.IndexOf(found);
                if (index != 0) Items.Move(index, 0);
            } else {
                var ptrn = new PatternItem(fpath);
                ptrn.OnSelected += OnSelect;
                Items.Insert(0, ptrn);
            }
        }

        private void CreateProject(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new SaveFileDialog {Filter = @"Pattern Project|*.ptrn"};
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) Open(dlg.FileName);
        }

        private void OpenProject(object sender, ExecutedRoutedEventArgs e) {
            var dlg = new OpenFileDialog {Filter = @"Pattern Project|*.ptrn"};
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) Open(dlg.FileName);
        }

        private void ExitBtn_OnClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void PatternsBrowser_OnClosing(object sender, CancelEventArgs e) {
            Save();
        }

        private readonly string _exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);
        private void Save() {
            var xDoc = new XDocument();
            var root = new XElement("PatternsScanner");
            xDoc.Add(root);

            if (OpenedPattern != null)
                root.Add(new XElement("OpenedPattern", OpenedPattern.ProjectPath));

            var projects = new XElement("Patterns");
            root.Add(projects);
            foreach (var item in Items)
                projects.Add(new XElement("Item", item.path.Text));

            xDoc.Save($"{_exePath}/patterns.pxml");
        }

        private string Load() {
            if (!File.Exists($"{_exePath}/patterns.pxml"))
                return null;
            var xDoc = XDocument.Load($"{_exePath}/patterns.pxml");
            var root = xDoc.Root;

            var ptrns = root?.Element("Patterns");
            if (ptrns != null) {
                foreach (var el in ptrns.Elements()) {
                    if (el.Name.LocalName == "Item") {
                        AddPath(el.Value);
                    }
                }
            }

            var opened = root?.Element("OpenedPattern");
            return opened?.Value;
        }
    }
}