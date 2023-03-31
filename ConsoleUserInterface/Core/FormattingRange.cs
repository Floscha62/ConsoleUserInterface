namespace ConsoleUserInterface.Core
{
    internal readonly struct FormattingRange {
        internal readonly (int column, int row) start;
        internal readonly (int column, int row) end;
        internal readonly IFormatting formatting;

        public static FormattingRange operator +(FormattingRange first, (int column, int row) offset) =>
            new (
                (first.start.column + offset.column, first.start.row + offset.row),
                (first.end.column + offset.column, first.end.row + offset.row),
                first.formatting
            );

        internal void Deconstruct(out (int, int) start, out (int, int) end, out IFormatting formatting) =>
            (start, end, formatting) = (this.start, this.end, this.formatting);

        internal FormattingRange((int column, int row) start, (int column, int row) end, IFormatting formatting) {
            this.start = start;
            this.end = end;
            this.formatting = formatting;
        }
    }
}
