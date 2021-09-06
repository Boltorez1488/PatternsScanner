using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatternsScanner {
    public class HeaderPattern {
        private readonly string _fin;
        private readonly string _fout;

        private string _readed;
        private MatchCollection _reps;
        private int _counter = 0;
        private int _offset = 0;

        public HeaderPattern(string fin, string fout) {
            _fin = fin;
            _fout = fout;
            Read();
        }

        public void Read() {
            if (string.IsNullOrEmpty(_fin) || !File.Exists(_fin))
                return;
            _readed = File.ReadAllText(_fin);
            _reps = Regex.Matches(_readed, "0x[0-9A-Fa-f]{0,16}");
        }

        public void Push(long address) {
            if (_reps == null)
                return;
            if (_counter >= _reps.Count)
                return;

            var build = $"0x{address:X8}";
            var match = _reps[_counter];

            var index = match.Index + _offset;
            _readed = _readed.Remove(index, match.Length).Insert(index, build);

            _offset += build.Length - match.Length;
            _counter++;
        }

        public void Save() {
            if (string.IsNullOrEmpty(_fout))
                return;
            File.WriteAllText(_fout, _readed);
        }
    }
}
