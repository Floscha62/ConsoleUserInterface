using System;

namespace ConsoleUserInterface.Core.Components {
    internal class Button : CompoundComponent<Button.Props, Button.State> {
        public record Props(Action OnClick, string Label, bool Underlined);
        public record State();

        public Button(Props props, ITransform transform) : base(props, transform) {
        }

        protected override State StartingState => new();

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) {
            if (keyInfo.Key == ConsoleKey.Enter) {
                props.OnClick?.Invoke();
                return true;
            }
            return false;
        }

        public override CompoundRenderResult Render(int width, int height, bool inFocus) => 
            new(Components.Label(this.Transform, props.Label, underlined: props.Underlined), inFocus);
    }
}
