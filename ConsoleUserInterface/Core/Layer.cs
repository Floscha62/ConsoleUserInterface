using ConsoleUserInterface.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core {


    public readonly struct FormattingRange {
        public readonly (int column, int row) start;
        public readonly (int column, int row) end;
        public readonly IFormatting formatting;

        public static FormattingRange operator +(FormattingRange first, (int column, int row) offset) =>
            new FormattingRange(
                (first.start.column + offset.column, first.start.row + offset.row),
                (first.end.column + offset.column, first.end.row + offset.row),
                first.formatting
            );

        public void Deconstruct(out (int, int) start, out (int, int) end, out IFormatting formatting) =>
            (start, end, formatting) = (this.start, this.end, this.formatting);

        public FormattingRange((int column, int row) start, (int column, int row) end, IFormatting formatting) {
            this.start = start;
            this.end = end;
            this.formatting = formatting;
        }
    }

    public interface IFormatting {

        private const string START_UNDERLINE = "\x1b[4m";
        private const string BACKGROUND_COLOR_FMTSTR = "\x1b[48;2;{0};{1};{2}m";
        private const string CLEAR_STYLE = "\x1b[0m";

        void Apply(IConsole console);

        class Formatting : IFormatting {
            private readonly Action<IConsole> format;

            public Formatting(Action<IConsole> format) {
                this.format = format;
            }

            public void Apply(IConsole console) {
                format(console);
            }
        }

        readonly static IFormatting Underlined = new Formatting(c => {
            c.Write(CLEAR_STYLE);
            c.Write(START_UNDERLINE);
        });
        readonly static IFormatting Blanked = new Formatting(c => {
            c.Write(CLEAR_STYLE);
        });

        static FormattingRange Underline((int, int) start, (int, int) end) =>
            new FormattingRange(start, end, Underlined);

        static FormattingRange Blank((int, int) start, (int, int) end) =>
            new FormattingRange(start, end, Blanked);

        static FormattingRange Background(int r, int g, int b, (int, int) start, (int, int) end) =>
            new FormattingRange(start, end, new Formatting(c => {
                c.Write(CLEAR_STYLE);
                c.Write(string.Format(BACKGROUND_COLOR_FMTSTR, r, g, b));
            }));
    }

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

        internal void Write(string input, int xOffset, int yOffset, IEnumerable<FormattingRange> formattings) {
            formattingRanges = formattingRanges.Merge(formattings, xOffset, yOffset, width);
            var lines = input.SplitLines();

            for (int y = 0; y < lines.Length; y++) {
                var line = lines[y];
                for (int i = 0; i < line.Length; i++) {
                    if (line[i] != '\0') {
                        layer[(yOffset + y) * width + xOffset + i] = line[i];
                    }
                }
            }
        }

        internal Layer MergeUp(Layer layerUp) {
            var l = new Layer(width, height, console);
            var raw = new string(this.layer);
            var rawUp = new string(layerUp.layer);

            l.Write(raw, 0, 0, this.formattingRanges);
            l.Write(rawUp, 0, 0, layerUp.formattingRanges);

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
                        f.formatting.Apply(console);
                        console.SetCursorPosition(f.start.column, row);
                        console.Write(padded[f.start.column..(f.end.column + 1)]);
                    }
                }
                row++;
            }
        }

        IEnumerable<string> Lines() {
            if (layer == null) {
                return new[] { "" };
            }
            var list = new List<string>();
            var l = new Span<char>(this.layer);
            for (var i = 0; i < height; i++) {
                var line = l.Slice(i * width, width);
                list.Add(new string(line).Replace('\0', ' '));
            }
            return list;
        }

        static (IEnumerable<string>, IEnumerable<string>) Longer(string[] first, string[] second) {
            var diff = first.Length - second.Length;
            if (diff == 0) {
                return (first, second);
            } else if (diff < 0) {
                var fList = first.ToList();
                fList.AddRange(Enumerable.Repeat("", -diff));
                return (fList, second);
            } else {
                var sList = second.ToList();
                sList.AddRange(Enumerable.Repeat("", diff));
                return (first, sList);
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
