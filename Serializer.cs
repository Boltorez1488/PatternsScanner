using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PatternsScanner {
    class Serializer {
        public static void Field(XElement root, Field field) {
            var el = new XElement("Field");
            root.Add(el);
            el.Add(new XElement("Left", field.Left));
            el.Add(new XElement("Comment", field.Comment));
        }

        public static void Pattern(XElement root, Pattern pattern) {
            var el = new XElement("Pattern");
            root.Add(el);
            el.Add(new XElement("Offset", pattern.Offset));
            el.Add(new XElement("CheckByte", pattern.CheckByte));
            el.Add(new XElement("NumberExtractor", pattern.NumberExtractor));
            el.Add(new XElement("PatternBytes", pattern.PatternBytes));
            el.Add(new XElement("Comment", pattern.Comment));
        }

        public static void Block(XElement root, Block block) {
            var el = new XElement("Block");
            root.Add(el);
            el.Add(new XElement("Name", block.MainName));
            el.Add(new XElement("IsExpanded", block.expander.IsExpanded));
            var items = new XElement("Items");
            foreach (var item in block.Items) {
                switch (item) {
                    case Block b:
                        Block(items, b);
                        break;
                    case Pattern p:
                        Pattern(items, p);
                        break;
                    case Field f:
                        Field(items, f);
                        break;
                }
            }
            el.Add(items);
        }

        public static void WinParams(XElement root, MainWindow window) {
            root.Add(new XElement("Top", Convert.ToInt32(window.Top)));
            root.Add(new XElement("Left", Convert.ToInt32(window.Left)));
            root.Add(new XElement("Width", Convert.ToInt32(window.Width)));
            root.Add(new XElement("Height", Convert.ToInt32(window.Height)));
            root.Add(new XElement("State", Convert.ToInt32(window.WindowState)));
        }

        public static void Serialize(MainWindow window, bool winParams = false) {
            Logger.Log("Serialization started");
            var xDoc = new XDocument();
            var root = new XElement("Pattern");
            xDoc.Add(root);

            if (winParams) {
                var win = new XElement("Window");
                WinParams(win, window);
                root.Add(win);
            }
            
            root.Add(new XElement("ScanFile", window.scanBox.Text));
            root.Add(new XElement("OutFile", window.outBox.Text));
            root.Add(new XElement("PatternFile", window.patternBox.Text));
            root.Add(new XElement("PatternOutFile", window.patternBuildBox.Text));
            root.Add(new XElement("ModBase", window.baseBox.Text));

            var items = new XElement("Items");
            foreach (var item in window.Items) {
                Block(items, item);
            }
            root.Add(items);

            xDoc.Save(window.ProjectPath);
            Logger.Log("Serialization finished");
        }
    }
}
