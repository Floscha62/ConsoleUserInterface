using System;
using System.Collections.Generic;
using static ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Core.Components {
    internal class TextArea : CompoundComponent<TextArea.Props, TextArea.State> {
        internal record Props(string StartingText, int BoxWidth, Action<string> OnChange, string Ellipsis = "...");
        internal record State(bool Open, string CurrentText, IComponent TextField);

        private static readonly ILogger logger = LoggingFactory.Create(typeof(TextArea));

        protected override State StartingState {
            get {
                var innerComp = Modal(this.Transform, TextField(ITransform.Create(props.BoxWidth, 1), props.StartingText, s => {
                    this.state = this.state with { CurrentText = s };
                }));
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
                logger.Debug($"Popup toggled: {state.Open} -> {!state.Open}");

                if (state.Open) props.OnChange?.Invoke(state.CurrentText);
                state = state with { Open = !state.Open };
                return true;
            }
            return state.Open && state.TextField.ReceiveKey(key);
        }

        public override CompoundRenderResult Render(int width, int height, bool inFocus) => new(RenderInternal(inFocus));

        IEnumerable<(IComponent, bool)> RenderInternal(bool inFocus) {
            yield return (Label(this.Transform, state.CurrentText, props.Ellipsis), inFocus && !state.Open);
            if (state.Open) {
                yield return (state.TextField, inFocus);
            }
        }
    }
}
