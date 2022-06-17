using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUserInterface.Core.Extensions;
using static ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Core.Components {
    internal class TextArea : VerticalComponent<TextArea.Props, TextArea.State> {
        internal record Props(string StartingText, int ShowLength, int BoxWidth, Action<string> OnChange, string Ellipsis = "...");
        internal record State(bool Open, string CurrentText, IComponent TextField);
        protected override State StartingState => new State(
            false,
            props.StartingText,
            Modal(Box(TextField(props.StartingText, s => {
                state = state with { CurrentText = s };
                props.OnChange(s);
            }, props.BoxWidth)), 1)
        );

        public TextArea(Props props) : base(props) { }

        public override bool ReceiveKey(ConsoleKeyInfo key) {
            switch (key) {
                case ConsoleKeyInfo(ConsoleKey.Enter, _, 0):
                    state = state with { Open = !state.Open };
                    return true;
                case var k when state.Open:
                    return state.TextField.ReceiveKey(k);
            }
            return false;
        }

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() =>
            (RenderInternal(), Enumerable.Empty<FormattingRange>());

        IEnumerable<IComponent> RenderInternal() {
            yield return Label(state.CurrentText.Ellipsis(props.Ellipsis, props.ShowLength));
            if (state.Open) {
                yield return state.TextField;
            }
        }
    }
}
