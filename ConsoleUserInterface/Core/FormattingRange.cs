namespace ConsoleUserInterface.Core
{
    public readonly struct FormattingRange {
        public readonly (int column, int row) start;
        public readonly (int column, int row) end;
        public readonly IFormatting formatting;

        public static FormattingRange operator +(FormattingRange first, (int column, int row) offset) =>
            new (
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
}
