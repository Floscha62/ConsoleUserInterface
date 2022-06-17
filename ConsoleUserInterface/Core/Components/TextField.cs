using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUserInterface.Core.Extensions;

namespace ConsoleUserInterface.Core.Components {
    internal class TextField : BaseComponent<TextField.Props, TextField.State> {
        internal record Props(string StartingText, Action<string> OnChange, int MaxWidth = -1);
        internal record State(string CurrentText, int CursorPosition);

        public override int Width => props.MaxWidth == -1 ? state.CurrentText.Insert(state.CursorPosition, "|")
            .SplitLines()
            .Max(s => s.Length) : props.MaxWidth;

        public override int Height => RenderString()
            .text
            .SplitLines()
            .Length;

        public TextField(Props props) : base(props) { }

        public override bool ReceiveKey(ConsoleKeyInfo key) {
            switch (key) {
                case ConsoleKeyInfo(ConsoleKey.LeftArrow, _, _) when state.CursorPosition > 0:
                    state = state with { CursorPosition = state.CursorPosition - 1 };
                    return true;
                case ConsoleKeyInfo(ConsoleKey.RightArrow, _, _) when state.CursorPosition < state.CurrentText.Length:
                    state = state with { CursorPosition = state.CursorPosition + 1 };
                    return true;
                case ConsoleKeyInfo(_, var c, _) when (char.IsLetterOrDigit(c) ||
                                               char.IsWhiteSpace(c) ||
                                               char.IsPunctuation(c) ||
                                               char.IsSymbol(c)) && c != '\r':
                    state = state with
                    {
                        CurrentText = state.CurrentText.Insert(state.CursorPosition, $"{c}"),
                        CursorPosition = state.CursorPosition + 1
                    };
                    props.OnChange(state.CurrentText);
                    return true;
                case ConsoleKeyInfo(ConsoleKey.Backspace, _, _) when state.CursorPosition > 0:
                    state = state with
                    {
                        CurrentText = state.CurrentText.Remove(state.CursorPosition - 1, 1),
                        CursorPosition = state.CursorPosition - 1
                    };
                    props.OnChange(state.CurrentText);
                    return true;
            }
            return false;
        }

        public override (string text, List<FormattingRange> formattings) RenderString() => (props.MaxWidth == -1 ? 
            state.CurrentText.Insert(state.CursorPosition, "|") :
            string.Join('\r', state.CurrentText.Insert(state.CursorPosition, "|")
                .Split(props.MaxWidth)
                .Select(s => s.PadRight(props.MaxWidth))
            ), new List<FormattingRange>());

        protected override State StartingState => new State(props.StartingText, props.StartingText.Length);
    }
}
