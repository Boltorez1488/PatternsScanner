using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PatternsScanner {
    public partial class MainWindow {
        public string ProjectPath;
        public PatternsBrowser Caller;
        public ObservableCollection<Block> Items = new ObservableCollection<Block>();

        public MainWindow(string projPath, PatternsBrowser caller) {
            Constants.MainWindow = this;
            Caller = caller;
            Caller.OpenedPattern = this;
            InitializeComponent();
            DataContext = this;
            items.ItemsSource = Items;

            Logger.LogBox = logger;
            ProjectPath = projPath;
            Title = $"Patterns Scanner [by Boltorez1488] ({ProjectPath})";

            Deserializer.Deserialize(this, true);
            Logger.Log($"{ProjectPath} loaded");
        }

        private void OnChildUp(Block sender) {
            var index = Items.IndexOf(sender);
            if (index != 0) {
                Items.Move(index, index - 1);
            }
        }

        private void OnChildDown(Block sender) {
            var index = Items.IndexOf(sender);
            if (index != Items.Count - 1) {
                Items.Move(index, index + 1);
            }
        }

        private void OnChildDelete(Block sender) {
            var index = Items.IndexOf(sender);
            Items.RemoveAt(index);
            sender.OnUp -= OnChildUp;
            sender.OnDown -= OnChildDown;
            sender.OnDelete -= OnChildDelete;
            if (index != 0 && index == Items.Count)
                index--;
            if (Items.Count == 0) {
                items.Focus();
            } else if (Items[index] is Block b) {
                b.scroll.Focus();
            }
        }

        public Block AddBlock() {
            var child = new Block();
            child.OnUp += OnChildUp;
            child.OnDown += OnChildDown;
            child.OnDelete += OnChildDelete;
            Items.Add(child);
            return child;
        }

        public void AttachBlock(Block child) {
            var b = new Block();
            b.OnUp += OnChildUp;
            b.OnDown += OnChildDown;
            b.OnDelete += OnChildDelete;
            b.MainName = child.MainName;
            Items.Add(b);
            foreach (var item in child.Items) {
                switch (item) {
                    case Block obj:
                        obj.OnDelete -= child.OnChildDelete;
                        
                        obj.OnDelete += b.OnChildDelete;
                        obj.ParentReplace(b);
                        break;
                }
                b.Items.Add(item);
            }
            if (b.Items.Count != 0) {
                b.dummy.Visibility = Visibility.Collapsed;
            }
        }

        private void AddBBtn_OnClick(object sender, RoutedEventArgs e) {
            AddBlock();
        }

        private void BlockAdder(object sender, ExecutedRoutedEventArgs e) {
            AddBlock();
        }

        private void Save(object sender, ExecutedRoutedEventArgs e) {
            var focus = Keyboard.FocusedElement;
            if(focus is TextBox tb)
                Utils.EnterText(tb);
            Serializer.Serialize(this, true);
        }

        private void Load(object sender, ExecutedRoutedEventArgs e) {
            Deserializer.Deserialize(this);
        }

        private HeaderPattern Build() {
            var fin = patternBox.Text;
            var fout = patternBuildBox.Text;

            var pos = fin.IndexOf("{exe}", StringComparison.OrdinalIgnoreCase);
            if (pos != -1) {
                var exeName = Path.GetFileNameWithoutExtension(scanBox.Text);
                fin = fin.Remove(pos, 5).Insert(pos, exeName);
            }

            pos = fout.IndexOf("{exe}", StringComparison.OrdinalIgnoreCase);
            if (pos != -1) {
                var exeName = Path.GetFileNameWithoutExtension(scanBox.Text);
                fout = fout.Remove(pos, 5).Insert(pos, exeName);
            }

            return new HeaderPattern(fin, fout);
        }

        private void InitScan() {
            string fin = "";
            string fout = "";
            string baseText = "";
            Dispatcher.Invoke(new Action(() => {
                fin = scanBox.Text;
                fout = outBox.Text;
                baseText = baseBox.Text;
            }));

            var pos = fout.IndexOf("{exe}", StringComparison.OrdinalIgnoreCase);
            if (pos != -1) {
                var exeName = Path.GetFileNameWithoutExtension(fin);
                fout = fout.Remove(pos, 5).Insert(pos, exeName);
            }

            var modBase = long.Parse(baseText, NumberStyles.HexNumber);
            HeaderPattern hp = null;
            Dispatcher.Invoke(new Action(() => {
                hp = Build();
            }));
            try {
                Scanner.Scan(Items, fin, fout, modBase, hp);
            } catch(Exception ex) {
                if (ex.InnerException is System.Threading.Tasks.TaskCanceledException) {
                    Dispatcher.Invoke(new Action(() => {
                        Logger.Log("Scanner stopped");
                        Scanner.Progressor.Close();
                    }));
                    return;
                }
                Dispatcher.Invoke(new Action(() => {
                    Logger.Log(ex.Message);
                    Scanner.Progressor.Close();
                }));
                return;
            }
            hp.Save();
        }

        private void Scan(object sender, ExecutedRoutedEventArgs e) {
            var fin = scanBox.Text;
            var fout = outBox.Text;

            if (string.IsNullOrEmpty(fin)) {
                MessageBox.Show("Scan path can not be empty", "Scan Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!File.Exists(fin)) {
                MessageBox.Show($"{fin} not found", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(fout)) {
                MessageBox.Show("Output path can not be empty", "Scan Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var pw = new ProgressWindow();
            Scanner.Progressor = pw;
            Scanner.Window = this;
            pw.SetTotalCount(Scanner.GetPatternsCount(Items));

            var baseText = baseBox.Text;
            //Thread thread = new Thread(() => InitScan(new ScanParams() {
            //    fin = fin,
            //    fout = fout,
            //    baseText = baseText
            //}));
            Thread thread = new Thread(InitScan);
            thread.Start();

            pw.ShowDialog();
            Scanner.Progressor = null;
        }

        private bool _projectReturn;
        private void CloseProject(object sender, ExecutedRoutedEventArgs e) {
            _projectReturn = true;
            Close();
        }

        private void ScanBrowse_OnClick(object sender, RoutedEventArgs e) {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                scanBox.Text = dlg.FileName;
            }
        }

        private void OutBrowse_OnClick(object sender, RoutedEventArgs e) {
            var dlg = new System.Windows.Forms.SaveFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                outBox.Text = dlg.FileName;
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e) {
            Serializer.Serialize(this, true);
            Logger.LogBox = null;
            Constants.MainWindow = null;
            if (!_projectReturn) {
                Caller.Close();
            } else {
                Caller.OpenedPattern = null;
                Caller.Show();
            }
        }

        private void LogClear_OnClick(object sender, RoutedEventArgs e) {
            logger.Clear();
        }

        private void ExitBtn_OnClick(object sender, RoutedEventArgs e) {
            Close();
        }

        private void About_OnClick(object sender, RoutedEventArgs e) {
            var dlg = new AboutWindow();
            dlg.ShowDialog();
        }

        private void HPatternBrowse_OnClick(object sender, RoutedEventArgs e) {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                patternBox.Text = dlg.FileName;
            }
        }

        private void HBuildBrowse_OnClick(object sender, RoutedEventArgs e) {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                patternBuildBox.Text = dlg.FileName;
            }
        }
    }
}