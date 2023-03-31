namespace ConsoleUserInterface.Core {
    internal interface IFormatting {

        private const string START_UNDERLINE = "\x1b[4m";
        private const string BACKGROUND_COLOR_FMTSTR = "\x1b[48;2;{0};{1};{2}m";
        private const string FOREGROUND_COLOR_FMTSTR = "\x1b[38;2;{0};{1};{2}m";
        private const string CLEAR_STYLE = "\x1b[0m";

        void Apply(IConsole console);

        internal class Formatting : IFormatting {
            private readonly Action<IConsole> format;

            public Formatting(Action<IConsole> format) {
                this.format = format;
            }

            public void Apply(IConsole console) {
                format(console);
            }
        }

        internal readonly static IFormatting AdditiveUnderline = new Formatting(c => {
            c.Write(START_UNDERLINE);
        });
        internal readonly static IFormatting OverridingUnderline = new Formatting(c => {
            c.Write(CLEAR_STYLE);
            c.Write(START_UNDERLINE);
        });
        internal readonly static IFormatting Blanked = new Formatting(c => {
            c.Write(CLEAR_STYLE);
        });

        internal static IFormatting Background(int r, int g, int b, bool additive) => new Formatting(c => {
            if (!additive) {
                c.Write(CLEAR_STYLE);
            }
            c.Write(string.Format(BACKGROUND_COLOR_FMTSTR, r, g, b));
        });
        internal static IFormatting Foreground(int r, int g, int b, bool additive) => new Formatting(c => {
            if (!additive) {
                c.Write(CLEAR_STYLE);
            }
            c.Write(string.Format(FOREGROUND_COLOR_FMTSTR, r, g, b));
        });

        internal static FormattingRange Underline((int, int) start, (int, int) end, bool additive = false) =>
            additive ? new(start, end, AdditiveUnderline) : new(start, end, OverridingUnderline);

        internal static FormattingRange Blank((int, int) start, (int, int) end) =>
            new(start, end, Blanked);

        internal static FormattingRange Background(int r, int g, int b, (int, int) start, (int, int) end, bool additive = false) =>
            new(start, end, Background(r, g, b, additive));

        internal static FormattingRange Foreground(int r, int g, int b, (int, int) start, (int, int) end, bool additive = false) =>
            new(start, end, Foreground(r, g, b, additive));
    }
}
