using System.Collections.Generic;
using System.Windows;

namespace PatternsScanner {
    public partial class ProgressWindow : Window {
        private uint m_count;
        private uint m_total;

        public bool IsCancel { get; private set; } = false;

        public ProgressWindow() {
            InitializeComponent();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e) {
            stopBtn.IsEnabled = false;
            stopBtn.Content = "Stopping...";
            IsCancel = true;
        }

        public void SetTotalCount(uint total) {
            m_total = total;
            progress.Maximum = total;
        }

        public void SetCurrentCount(uint count) {
            m_count = count;
            update();
        }

        public void AddCount() {
            m_count++;
            update();
        }

        private void update() {
            progress.Value = m_count;
            counter.Content = $"{m_count}/{m_total}";
            //var path = GetCurrentPath();
            //if (!string.IsNullOrEmpty(path)) {
            //    scanText.Content = $"Scanning {GetCurrentPath()}";
            //}
            //if (m_blocks.Count != 0 && !string.IsNullOrEmpty(m_item)) {
            //    var str = "Scanning ";
            //    foreach(var b in m_blocks) {
            //        str += $"[{b}]";
            //    }
            //    str += $"[{m_item}]";
            //    scanText.Content = str;
            //}
        }
    }
}
