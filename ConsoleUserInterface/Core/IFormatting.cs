using System;

namespace ConsoleUserInterface.Core
{
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
            new(start, end, Underlined);

        static FormattingRange Blank((int, int) start, (int, int) end) =>
            new(start, end, Blanked);

        static FormattingRange Background(int r, int g, int b, (int, int) start, (int, int) end) =>
            new(start, end, new Formatting(c => {
                c.Write(CLEAR_STYLE);
                c.Write(string.Format(BACKGROUND_COLOR_FMTSTR, r, g, b));
            }));
    }
}
