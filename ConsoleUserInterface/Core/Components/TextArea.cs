using ConsoleUserInterface.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using static ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Core.Components {
    internal class TextArea : CompoundComponent<TextArea.Props, TextArea.State> {
        internal record Props(string StartingText, int ShowLength, int BoxWidth, Action<string> OnChange, string Ellipsis = "...");
        internal record State(bool Open, string CurrentText, IComponent TextField);
        protected override State StartingState {
            get {
                var innerComp = Modal(this.Transform, TextField(props.StartingText, s => {
                    this.state = this.state with { CurrentText = s };
                    props.OnChange?.Invoke(s);
                }, 50, 50, props.BoxWidth, 1));
                innerComp.ReceiveKey(new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false));
                return new(
                    false,
                    props.StartingText,
                    innerComp
                );
            }
        }

        public TextArea(Props props, ITransform transform) : base(props, transform) { }

        public override bool ReceiveKey(ConsoleKeyInfo key) {
            if (key.Key == ConsoleKey.Enter && key.Modifiers == 0) {
                state = state with { Open = !state.Open };
                return true;
            }
            return state.Open && state.TextField.ReceiveKey(key);
        }

        public override CompoundRenderResult Render(int width, int height) =>
            new(RenderInternal(), Enumerable.Empty<FormattingRange>());

        IEnumerable<IComponent> RenderInternal() {
            yield return Label(state.CurrentText.Ellipsis(props.Ellipsis, props.ShowLength), this.Transform);
            if (state.Open) {
                yield return state.TextField;
            }
        }
    }
}
