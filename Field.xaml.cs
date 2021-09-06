using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Field : INotifyPropertyChanged {
        public Block MainParent = null;

        private string _left;
        public string Left {
            get => _left;
            set {
                _left = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public delegate void UpEvent(Field sender);
        public delegate void DownEvent(Field sender);
        public delegate void DeleteEvent(Field sender);

        public event UpEvent OnUp;
        public event DownEvent OnDown;
        public event DeleteEvent OnDelete;

        public Field() {
            InitializeComponent();
            DataContext = this;

            Left = "???";
            Comment = "Description";
        }

        public Field(string left, string comment) {
            InitializeComponent();
            DataContext = this;

            Left = left;
            Comment = comment;
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

        private void delBtn_Click(object sender, RoutedEventArgs e) {
            Delete();
        }

        private void upBtn_Click(object sender, RoutedEventArgs e) {
            Up();
        }

        private void downBtn_Click(object sender, RoutedEventArgs e) {
            Down();
        }

        private void Field_OnLoaded(object sender, RoutedEventArgs e) {
            field.Focus();
            field.SelectAll();
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

        private void MoveBtn_OnClick(object sender, RoutedEventArgs e) {
            var dlg = new ElementMover.MainDialog(this, MainParent);
            if (dlg.ShowDialog() == true) {
                if (dlg.Selected != null) {
                    var b = dlg.Selected;
                    OnDelete?.Invoke(this);
                    b.AttachField(this);
                }
            }
        }
    }
}
