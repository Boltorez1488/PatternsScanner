using System.Collections.Generic;
using System.IO;

namespace PatternsScanner {
    internal class Memory {
        private readonly byte[] _bytes;

        public Memory(string exeName) {
            _bytes = File.ReadAllBytes(exeName);
        }

        public List<int> SearchPattern(byte[] bytes, string mask) {
            List<int> addresses = new List<int>();
            for (var i = 0; i < _bytes.Length; i++) {
                var match = 0;
                for (int j = i, k = 0; k < bytes.Length && j < _bytes.Length; j++, k++)
                    if (mask[k] == '?')
                        match++;
                    else if (bytes[k] == _bytes[j])
                        match++;
                    else
                        break;

                if (match == bytes.Length) {
                    addresses.Add(i);
                    i += 1;//bytes.Length; // That next bytes may be valid
                }
            }

            return addresses;
        }
    }
}