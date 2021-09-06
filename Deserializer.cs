using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace PatternsScanner {
    class Deserializer {
        public static void Field(XElement root, Block block) {
            var field = block.AddField();
            foreach (var el in root.Elements()) {
                switch (el.Name.LocalName) {
                    case "Left":
                        field.Left = el.Value;
                        break;
                    case "Comment":
                        field.Comment = el.Value;
                        break;
                }
            }
        }

        public static void Pattern(XElement root, Block block) {
            var pattern = block.AddPattern();
            foreach (var el in root.Elements()) {
                switch (el.Name.LocalName) {
                    case "Offset":
                        pattern.Offset = int.Parse(el.Value);
                        break;
                    case "CheckByte":
                        pattern.CheckByte = el.Value;
                        break;
                    case "NumberExtractor":
                        pattern.NumberExtractor = int.Parse(el.Value);
                        break;
                    case "PatternBytes":
                        pattern.PatternBytes = el.Value;
                        break;
                    case "Comment":
                        pattern.Comment = el.Value;
                        break;
                }
            }
        }

        public static void Block(XElement root, MainWindow window) {
            var block = window.AddBlock();
            var isExpanded = block.expander.IsExpanded;
            foreach (var el in root.Elements()) {
                switch (el.Name.LocalName) {
                    case "Name":
                        block.MainName = el.Value;
                        break;
                    case "IsExpanded":
                        isExpanded = bool.Parse(el.Value);
                        break;
                    case "Items":
                        foreach (var elem in el.Elements())
                            switch (elem.Name.LocalName) {
                                case "Block":
                                    Block(elem, block);
                                    break;
                                case "Pattern":
                                    Pattern(elem, block);
                                    break;
                                case "Field":
                                    Field(elem, block);
                                    break;
                            }
                        break;
                }
            }
            block.expander.IsExpanded = isExpanded;
        }

        public static void Block(XElement root, Block parent) {
            var block = parent.AddBlock();
            var isExpanded = block.expander.IsExpanded;
            foreach (var el in root.Elements()) {
                switch (el.Name.LocalName) {
                    case "Name":
                        block.MainName = el.Value;
                        break;
                    case "IsExpanded":
                        isExpanded = bool.Parse(el.Value);
                        break;
                    case "Items":
                        foreach (var elem in el.Elements())
                            switch (elem.Name.LocalName) {
                                case "Block":
                                    Block(elem, block);
                                    break;
                                case "Pattern":
                                    Pattern(elem, block);
                                    break;
                                case "Field":
                                    Field(elem, block);
                                    break;
                            }
                        break;
                }
            }

            block.expander.IsExpanded = isExpanded;
        }

        public static void WinParams(XElement root, MainWindow window) {
            foreach (var el in root.Elements()) {
                try {
                    switch (el.Name.LocalName) {
                        case "Top":
                            window.Top = Convert.ToDouble(el.Value);
                            break;
                        case "Left":
                            window.Left = Convert.ToDouble(el.Value);
                            break;
                        case "Width":
                            window.Width = Convert.ToDouble(el.Value);
                            break;
                        case "Height":
                            window.Height = Convert.ToDouble(el.Value);
                            break;
                        case "State":
                            if (Enum.TryParse(el.Value, out WindowState ws))
                                window.WindowState = ws;
                            break;
                    }
                } catch {
                    // ignored
                }
            }
        }

        public static void Deserialize(MainWindow window, bool winParams = false) {
            if (!File.Exists(window.ProjectPath))
                return;
            var xDoc = XDocument.Load(window.ProjectPath);
            var root = xDoc.Root;
            if (root == null)
                return;

            Logger.Log("Deserialization started");
            while (window.Items.Count > 0)
                window.Items.Last().Delete();

            foreach (var el in root.Elements()) {
                switch (el.Name.LocalName) {
                    case "Window":
                        if(winParams)
                            WinParams(el, window);
                        break;
                    case "ScanFile":
                        window.scanBox.Text = el.Value;
                        break;
                    case "OutFile":
                        window.outBox.Text = el.Value;
                        break;
                    case "PatternFile":
                        window.patternBox.Text = el.Value;
                        break;
                    case "PatternOutFile":
                        window.patternBuildBox.Text = el.Value;
                        break;
                    case "ModBase":
                        window.baseBox.Text = el.Value;
                        break;
                    case "Items":
                        foreach(var elem in el.Elements())
                            if(elem.Name.LocalName == "Block")
                                Block(elem, window);
                        break;
                }
            }
            Logger.Log("Deserialization finished");
        }
    }
}

