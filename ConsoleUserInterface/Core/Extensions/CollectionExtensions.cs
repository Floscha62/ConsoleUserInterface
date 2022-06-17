using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core.Extensions {

    internal static class CollectionExtensions {

        internal static void Add<K, V>(this Dictionary<K, List<V>> dict, K key, V value) {
            if (!dict.ContainsKey(key)) {
                dict.Add(key, new List<V>());
            }

            dict[key].Add(value);
        }

        class AggregateState {
            public List<FormattingRange> Ranges => ranges;

            readonly List<FormattingRange> ranges = new List<FormattingRange>();
            (int column, int row) start = (-1, -1);
            (int column, int row) end = (-1, -1);
            IFormatting last;

            public AggregateState Apply(FormattingRange next, int width) {
                if (last is null || start == next.start && end == next.end) {
                    (start, end, last) = next;
                    return this;
                }

                var nextStartLater = end.row < next.start.row || (end.row == next.start.row && end.column <= next.start.column);
                if (nextStartLater) {
                    ranges.Add(new FormattingRange(start, end, last));
                    (start, end, last) = next;
                    return this;
                }

                var nextEndsLater = end.row < next.end.row || (end.row == next.end.row && end.column <= next.end.column);
                if (Equals(last, next.formatting)) {
                    end = nextEndsLater ? next.end : end;
                    return this;
                }
                var beforeNext = next.start.column == 0 ? (width - 1, next.start.row - 1) : (next.start.column - 1, next.start.row);
                var afterNext = next.end.column == width - 1 ? (0, next.end.row + 1) : (next.end.column + 1, next.start.row);
                if (nextEndsLater) {
                    ranges.Add(new FormattingRange(start, beforeNext, last));
                    (start, end, last) = next;
                    return this;

                }
                if (next.start != start) {
                    ranges.Add(new FormattingRange(start, beforeNext, last));
                }
                ranges.Add(next);
                start = afterNext;
                return this;
            }

            public AggregateState Finish() {
                if (!(last is null)) {
                    ranges.Add(new FormattingRange(start, end, last));
                }
                return this;
            }
        }

        internal static List<FormattingRange> Merge(this IEnumerable<FormattingRange> ranges, IEnumerable<FormattingRange> others, int xOff, int yOff, int width) {
            return ranges.Concat(others.Select(r => r + (xOff, yOff)))
                .OrderBy(r => r.start.row)
                .ThenBy(r => r.start.column)
                .Aggregate(new AggregateState(), (state, next) => state.Apply(next, width))
                .Finish()
                .Ranges;
        }
    }
}
