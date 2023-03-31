using ConsoleUserInterface.Core.Extensions;

namespace ConsoleUserInterface.Core.Components {
    internal class TextField : BaseComponent<TextField.Props, TextField.State> {
        internal TextField(Props props, ITransform transform) : base(props, transform) {
            CurrentState = new(props.InitialText, props.InitialText.Length);
        }

        internal record Props(string InitialText, Action<string> OnChange) {
            public override string ToString() => $"Props {{ InitialText = \"{InitialText}\" }}";
        }
        internal record State(string Text, int CursorPosition) { public State() : this("", 0) { } };

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) {
            switch (keyInfo) {
                case ConsoleKeyInfo(ConsoleKey.LeftArrow, _, _) when CurrentState.CursorPosition > 0:
                    CurrentState = CurrentState with { CursorPosition = CurrentState.CursorPosition - 1 };
                    return true;
                case ConsoleKeyInfo(ConsoleKey.RightArrow, _, _) when CurrentState.CursorPosition < CurrentState.Text.Length:
                    CurrentState = CurrentState with { CursorPosition = CurrentState.CursorPosition + 1 };
                    return true;
                case ConsoleKeyInfo(_, var c, _) when (char.IsLetterOrDigit(c) ||
                                               char.IsWhiteSpace(c) ||
                                               char.IsPunctuation(c) ||
                                               char.IsSymbol(c)) && c != '\t':
                    CurrentState = CurrentState with {
                        Text = CurrentState.Text.Insert(CurrentState.CursorPosition, $"{c}"),
                        CursorPosition = CurrentState.CursorPosition + 1
                    };
                    props.OnChange?.Invoke(CurrentState.Text);
                    return true;
                case ConsoleKeyInfo(ConsoleKey.Backspace, _, _) when CurrentState.CursorPosition > 0:
                    CurrentState = CurrentState with {
                        Text = CurrentState.Text.Remove(CurrentState.CursorPosition - 1, 1),
                        CursorPosition = CurrentState.CursorPosition - 1
                    };
                    props.OnChange?.Invoke(CurrentState.Text);
                    return true;
            }
            return false;
        }

        public override BaseRenderResult Render() => new(CurrentState.Text.Insert(CurrentState.CursorPosition, "|"));
    }
}
