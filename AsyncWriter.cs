using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsScanner {
    public class AsyncWriter {
        public static string Tab(int count = 0) {
            return Utils.SymbolGenerate('\t', count);
        }

        public static void Write(string fout, List<string> lines) {
            using (var stream = new StreamWriter(new FileStream(fout, FileMode.Create))) {
                for (var i = 0; i < lines.Count; i++)
                    if (i == lines.Count - 1)
                        stream.Write(lines[i]);
                    else
                        stream.WriteLine(lines[i]);
            }
        }

        private static void Field(List<string> build, Field root, int tabsCount, HeaderPattern hp) {
            var tab = Tab(tabsCount);
            if (string.IsNullOrEmpty(root.Comment)) {
                build.Add($"{tab}{root.Left}");
            } else if (string.IsNullOrEmpty(root.Left)) {
                build.Add($"{tab}{root.Comment}");
            } else {
                build.Add($"{tab}{root.Left} - {root.Comment}");
            }
            hp?.Push(0);
        }

        private static void Pattern(List<string> build, long modBase, Pattern root, int tabsCount, HeaderPattern hp) {
            var tab = Tab(tabsCount);
            var search = root.LastSearch;
            long getted = 0;
            if (root.IsSearchSuccess) {
                string res = "";
                var last = search.Last();
                //var count = 0;
                foreach (var a in search) {
                    long gen = a;
                    gen += root.Offset;
                    gen += modBase;
                    //if (root.NumberExtractor == 0) {
                    //    if (getted == 0)
                    //        getted = gen;
                    //    res += $"{gen:X8}";
                    //    if (last != a)
                    //        res += ",";
                    //} else {
                    //    count++;
                    //    if (count == root.NumberExtractor) {
                    //        if (getted == 0)
                    //            getted = gen;
                    //        res = $"{gen:X8}";
                    //        break;
                    //    }
                    //}
                    if (getted == 0)
                        getted = gen;
                    res += $"{gen:X8}";
                    if (last != a)
                        res += ",";
                }

                res = res.TrimEnd(',');

                if (getted == 0) {
                    long g = search.First();
                    g += root.Offset;
                    g += modBase;
                    hp?.Push(g); // HeaderPattern
                } else {
                    hp.Push(getted);
                }

                build.Add(!string.IsNullOrEmpty(root.Comment)
                    ? $"{tab}{res} - {root.Comment}"
                    : $"{tab}{res}");
            } else {
                hp?.Push(0); // HeaderPattern
                build.Add(!string.IsNullOrEmpty(root.Comment)
                    ? $"{tab}??? - {root.Comment}"
                    : $"{tab}???");
            }
        }

        private static void Block(List<string> build, long modBase, Block root, int tabsCount, HeaderPattern hp) {
            var tab = Tab(tabsCount);
            build.Add($"{tab}[{root.MainName}]: {{");
            var first = root.Items.First();
            var last = root.Items.Last();

            foreach (var item in root.Items) {
                switch (item) {
                    case Block b:
                        if (item != first)
                            build.Add("");
                        Block(build, modBase, b, tabsCount + 1, hp);
                        if (item != last)
                            build.Add("");
                        break;
                    case Pattern p:
                        Pattern(build, modBase, p, tabsCount + 1, hp);
                        break;
                    case Field f:
                        Field(build, f, tabsCount + 1, hp);
                        break;
                }
            }
            build.Add($"{tab}}}");
        }

        public static void WriteAll(ObservableCollection<Block> items, string fin, string fout, long modBase, HeaderPattern hp) {
            Scanner.Window.Dispatcher.Invoke(() => {
                Logger.Log($"Writting {fout}");
            });

            var build = new List<string>();
            var last = items.Last();
            foreach (var item in items) {
                if (item is Block b) {
                    Block(build, modBase, b, 0, hp);
                    if (!Equals(item, last))
                        build.Add("");
                }
            }

            Write(fout, build);
            Scanner.Window.Dispatcher.Invoke(() => {
                Logger.Log($"{fout} writted");
                Logger.Log("Scanner finished");
            });
        }
    }
}
