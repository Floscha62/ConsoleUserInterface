namespace LoggingConsole {

    internal static class TextStyleExtension {

        internal static IEnumerable<string> Split(this string str, int n) {
            if (string.IsNullOrEmpty(str) || n < 1) {
                throw new ArgumentException("");
            }

            var rest = str;

            while (rest != "") {
                var newLine = rest.IndexOfAny(new[] { '\n', '\r' });
                if (newLine >= n || newLine == -1) {
                    if (rest.Length <= n) {
                        yield return rest;
                        yield break;
                    }
                    var space = rest.LastIndexOf(' ', n);
                    if (space == -1) {
                        yield return rest[..n];
                        rest = rest[n..];
                    } else {
                        yield return rest[..space];
                        rest = rest[(space + 1)..];
                    }
                } else {
                    yield return rest[..newLine];
                    rest = rest[(newLine + 1)..];
                }
            }
        }

        internal static string Ellipsis(this string str, string ellipsis, int maxText) {
            if (str.Length <= maxText) {
                return str;
            } else {
                return $"{str[0..(maxText - ellipsis.Length)]}{ellipsis}";
            }
        }
    }
    internal struct Layer {
        private readonly char[] layer;
        private readonly int width;
        private readonly int height;

        internal Layer(int width, int height) {
            layer = new char[width * height];
            this.width = width;
            this.height = height;
        }

        internal void Write(string input, int xOffset, int yOffset, int w, int h) {
            var lines = input.Split(w).ToArray();
            var longest = lines.Max(l => l.Length);
            for (int y = 0; y < Math.Min(lines.Length, h); y++) {
                var line = lines[y];
                for (int i = 0; i < Math.Min(line.Length, w); i++) {
                    var layerIndex = (yOffset + y) * width + xOffset + i;
                    if (line[i] != '\0' && layerIndex >= 0 && layerIndex < layer.Length && i < line.Length) {
                        layer[layerIndex] = line[i];
                    }
                }
            }
        }

        internal Layer MergeUp(Layer layerUp) {
            var l = new Layer(width, height);

            var i = 0;
            foreach (var (raw, rawUp) in this.Lines(false).Zip(layerUp.Lines(false))) {
                l.Write(raw, 0, i, width, height);
                l.Write(rawUp, 0, i, width, height);
                i++;
            }

            return l;
        }

        internal void PrintToConsole(Layer last, bool force) {
            var lastLines = last.Lines().ToArray();
            var currentLines = Lines().ToArray();

            (var lines, var oldLines) = Longer(currentLines, lastLines);
            Console.BufferHeight = Math.Max(lines.Count(), Console.BufferHeight);

            var row = 0;
            foreach (var (line, lastLine) in Enumerable.Zip(lines, oldLines)) {
                if (line != lastLine || force) {
                    Console.SetCursorPosition(0, row);
                    var padded = line.PadRight(lastLine.Length);
                    Console.Write(padded);
                }
                row++;
            }
        }

        IEnumerable<string> Lines(bool replaceNull = true) {
            if (layer == null) return new[] { "" };

            var list = new List<string>();
            var l = new Span<char>(layer);
            for (var i = 0; i < height; i++) {
                var line = new string(l.Slice(i * width, width));
                if (replaceNull)
                    line = line.Replace('\0', ' ');

                list.Add(line);
            }
            return list;
        }

        static (IEnumerable<string>, IEnumerable<string>) Longer(string[] first, string[] second) {
            var diff = first.Length - second.Length;
            switch (diff) {
                case 0:
                    return (first, second);
                case < 0: {
                        var fList = first.ToList();
                        fList.AddRange(Enumerable.Repeat("", -diff));
                        return (fList, second);
                    }
                default: {
                        var sList = second.ToList();
                        sList.AddRange(Enumerable.Repeat("", diff));
                        return (first, sList);
                    }
            }
        }
    }
}
