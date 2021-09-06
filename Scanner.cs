using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PatternsScanner {
    static class Scanner {
        public static MainWindow Window = null;
        public static ProgressWindow Progressor = null;

        private static bool CheckByte(Pattern root, long addr) {
            if (string.IsNullOrEmpty(root.CheckByte))
                return true;
            var str = addr.ToString("X");
            byte last = byte.Parse(str.Last() + "", NumberStyles.HexNumber);
            byte check = byte.Parse(root.CheckByte, NumberStyles.HexNumber);
            if (check != last)
                return false;

            return true;
        }

        private static CancellationTokenSource token;
        private static List<Task> patterns = new List<Task>();

        private static void Pattern(Memory mem, long modBase, Pattern root, int tabsCount) {
            var p = root.Build();
            if (Progressor.IsCancel) {
                token.Cancel();
                throw new Exception("Scanner stoped");
            }
            if (root.IsSearchSuccess) {
                Window.Dispatcher.Invoke(() => {
                    if (!Progressor.IsCancel)
                        Progressor.AddCount();
                    //Logger.Log($"{root.GetPath()} already found, skipped");
                });
                return;
            }
            var search = mem.SearchPattern(p.Bytes, p.Mask);
            token.Token.ThrowIfCancellationRequested();
            int count = 0;
            if (search.Count != 0) {
                var successList = new List<int>();
                var last = search.Last();
                var warnList = new List<string>();
                bool isSuccess = false;
                foreach (var a in search) {
                    long gen = a;
                    gen += root.Offset;
                    gen += modBase;
                    if (root.NumberExtractor != 0) {
                        count++;
                        if (count == root.NumberExtractor) {
                            if (!CheckByte(root, gen)) {
                                warnList.Add($"Warning - [{gen:X} != {root.CheckByte}]{root.GetPath()} last byte check is failed");
                            } else {
                                successList.Add(a);
                                isSuccess = true;
                            }
                            break;
                        } else {
                            continue;
                        }
                    }
                    if (!CheckByte(root, gen)) {
                        warnList.Add($"Warning - [{gen:X} != {root.CheckByte}]{root.GetPath()} last byte check is failed");
                    } else {
                        successList.Add(a);
                        isSuccess = true;
                    }
                }
                if (!isSuccess) {
                    Window.Dispatcher.Invoke(() => {
                        foreach (var w in warnList) {
                            Logger.Log(w);
                        }
                    });
                } else {
                    root.IsSearchSuccess = true;
                    root.LastSearch = successList;
                }
            } else {
                Window.Dispatcher.Invoke(() => {
                    Logger.Log($"Error - {root.GetPath()} - not found");
                });
            }

            token.Token.ThrowIfCancellationRequested();
            Window.Dispatcher.Invoke(() => {
                //Progressor.SetItem(string.IsNullOrEmpty(root.Comment) ? "Pattern" : root.Comment);
                if (!Progressor.IsCancel)
                    Progressor.AddCount();
            });
        }

        private static void Block(Memory mem, long modBase, Block root, int tabsCount) {
            var first = root.Items.First();
            var last = root.Items.Last();
            if (Progressor.IsCancel) {
                token.Cancel();
                throw new Exception("Scanner stopped");
            }
            foreach (var item in root.Items) {
                switch (item) {
                    case Block b:
                        Block(mem, modBase, b, tabsCount + 1);
                        break;
                    case Pattern p:
                        patterns.Add(Task.Factory.StartNew(() => {
                            try {
                                Pattern(mem, modBase, p, tabsCount + 1);
                            } catch(Exception) {
                                //...
                            }
                        }, token.Token));
                        break;
                    case Field f:
                        break;
                }
            }
            if (Progressor.IsCancel) {
                token.Cancel();
                throw new Exception("Scanner stopped");
            }
        }

        private static void countBlock(Block root, ref uint counter) {
            foreach(var item in root.Items) {
                switch (item) {
                    case Block b:
                        countBlock(b, ref counter);
                        break;
                    case Pattern _:
                        counter++;
                        break;
                }
            }
        }

        public static uint GetPatternsCount(ObservableCollection<Block> items) {
            uint counter = 0;
            foreach(var item in items) {
                if (item is Block b) {
                    countBlock(b, ref counter);
                }
            }
            return counter;
        }

        public static void Scan(ObservableCollection<Block> items, string fin, string fout, long modBase, HeaderPattern hp = null) {
            if (items.Count == 0) {
                Window.Dispatcher.Invoke(() => {
                    Logger.Log("Warning - Scan body is empty");
                    Logger.Log("Scanner finished");
                });
                return;
            }
            
            token = new CancellationTokenSource();
            patterns.Clear();

            Window.Dispatcher.Invoke(() => {
                Logger.Log("Scanner started");
            });
            var mem = new Memory(fin);
            var last = items.Last();
            foreach (var item in items) {
                if (item is Block b) {
                    Block(mem, modBase, b, 0);
                }
            }

            if (token.IsCancellationRequested) {
                throw new Exception("Scanner stopped");
            }
            Task.WaitAll(patterns.ToArray());

            if (Progressor.IsCancel)
                throw new Exception("Scanner stopped");

            AsyncWriter.WriteAll(items, fin, fout, modBase, hp);

            Window.Dispatcher.Invoke(() => {
                Progressor.Close();
            });
        }
    }
}
