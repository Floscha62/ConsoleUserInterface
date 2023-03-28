using ConsoleUserInterface.Core.Extensions;

namespace ConsoleUserInterface.Core {
    internal struct Layer {
        private readonly char[] layer;
        private readonly int width;
        private readonly int height;
        private readonly IConsole console;

        private List<FormattingRange> formattingRanges;

        internal Layer(int width, int height, IConsole console) {
            layer = new char[width * height];
            this.width = width;
            this.height = height;
            this.console = console;
            formattingRanges = new List<FormattingRange>();
        }

        internal void Write(string input, int xOffset, int yOffset, int w, int h) {
            var lines = input.Split(w).ToArray();
            var longest = lines.Max(l => l.Length);
            for (int y = 0; y < h; y++) {
                var line = y < lines.Length ? lines[y] : "";
                for (int i = 0; i < w; i++) {
                    var layerIndex = (yOffset + y) * width + xOffset + i;
                    if (i < line.Length) {
                        if (line[i] != '\0' && layerIndex >= 0 && layerIndex < layer.Length && i < line.Length) {
                            layer[layerIndex] = line[i];
                        }
                    } else if(layerIndex >= 0 && layerIndex < layer.Length) layer[layerIndex] = ' ';
                }
            }
        }

        internal void ApplyFormatting(int xOffset, int yOffset, IEnumerable<FormattingRange> formattings) =>
            formattingRanges = formattingRanges.Merge(formattings, xOffset, yOffset, width);

        internal Layer MergeUp(Layer layerUp) {
            var l = new Layer(width, height, console);

            var i = 0;
            foreach (var (raw, rawUp) in this.Lines(false).Zip(layerUp.Lines(false))) {
                l.Write(raw, 0, i, width, height);
                l.Write(rawUp, 0, i, width, height);
                i++;
            }

            l.ApplyFormatting(0, 0, this.formattingRanges);
            l.ApplyFormatting(0, 0, layerUp.formattingRanges);

            return l;
        }

        internal void PrintToConsole(Layer last, bool force) {
            var lastLines = last.Lines().ToArray();
            var currentLines = Lines().ToArray();

            (var lines, var oldLines) = Longer(currentLines, lastLines);
            console.BufferHeight = Math.Max(lines.Count(), console.BufferHeight);

            formattingRanges = BreakIntoLines(formattingRanges, width).ToList();

            var row = 0;
            foreach (var (line, lastLine) in Enumerable.Zip(lines, oldLines)) {
                if (line != lastLine || ChangeInLine(last.formattingRanges, formattingRanges, row) || force) {
                    var padded = line.PadRight(lastLine.Length);
                    foreach (var f in formattingRanges.Where(r => r.start.row == row)) {
                        console.SetCursorPosition(f.start.column, row);
                        f.formatting.Apply(console);
                        console.Write(padded[f.start.column..(f.end.column + 1)]);
                    }
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


        static IEnumerable<FormattingRange> BreakIntoLines(List<FormattingRange> ranges, int width) {
            foreach (var range in ranges) {
                if (range.start.row == range.end.row) {
                    yield return range;
                } else {
                    yield return new FormattingRange(range.start, (width - 1, range.start.row), range.formatting);
                    for (int i = 1; i < range.end.row - range.start.row; i++) {
                        yield return new FormattingRange((0, range.start.row + i), (width - 1, range.start.row + i), range.formatting);
                    }
                    yield return new FormattingRange((0, range.end.row), range.end, range.formatting);
                }
            }
        }

        static bool ChangeInLine(List<FormattingRange> last, List<FormattingRange> current, int line) {
            var lastRangesInLine = last.Where(r => r.start.row == line || r.end.row == line);
            var currentRangesInLine = current.Where(r => r.start.row == line || r.end.row == line);
            if (lastRangesInLine.Count() != currentRangesInLine.Count()) return true;
            if (!lastRangesInLine.Any()) return false;

            var ranges = lastRangesInLine.Zip(currentRangesInLine);
            var (firstPrevious, firstCurrent) = ranges.First();
            var (lastPrevious, lastCurrent) = ranges.Last();
            var middleRange = ranges.Skip(1).Take(lastRangesInLine.Count() - 2);

            return (firstPrevious.end == firstCurrent.end && Equals(firstPrevious, firstCurrent)) &&
                middleRange.All(tuple => tuple.First.start == tuple.Second.start && tuple.First.end == tuple.Second.end && Equals(tuple.First.formatting, tuple.Second.formatting)) &&
                lastPrevious.start == lastCurrent.start && Equals(firstPrevious, firstCurrent);

        }
    }
}
