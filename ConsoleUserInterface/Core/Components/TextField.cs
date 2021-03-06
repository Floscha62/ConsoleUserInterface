using ConsoleUserInterface.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class TextField : BaseComponent<TextField.Props, TextField.State> {
        internal record Props(string StartingText, Action<string> OnChange, int MaxWidth = -1);
        internal record State(string CurrentText, int CursorPosition);

        public TextField(Props props, ITransform transform) : base(props, transform) { }

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
                    state = state with {
                        CurrentText = state.CurrentText.Insert(state.CursorPosition, $"{c}"),
                        CursorPosition = state.CursorPosition + 1
                    };
                    props.OnChange?.Invoke(state.CurrentText);
                    return true;
                case ConsoleKeyInfo(ConsoleKey.Backspace, _, _) when state.CursorPosition > 0:
                    state = state with {
                        CurrentText = state.CurrentText.Remove(state.CursorPosition - 1, 1),
                        CursorPosition = state.CursorPosition - 1
                    };
                    props.OnChange?.Invoke(state.CurrentText);
                    return true;
            }
            return false;
        }

        public override BaseRenderResult Render(int width, int height) => new(props.MaxWidth == -1 ?
            state.CurrentText.Insert(state.CursorPosition, "|") :
            string.Join('\r', state.CurrentText.Insert(state.CursorPosition, "|")
                .Split(props.MaxWidth)
                .Select(s => s.PadRight(props.MaxWidth))
            ), new List<FormattingRange>());

        protected override State StartingState => new (props.StartingText, props.StartingText.Length);
    }
}
