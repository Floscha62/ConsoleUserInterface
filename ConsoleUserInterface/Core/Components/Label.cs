using System;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class Label : BaseComponent<Label.Props, Label.State> {

        public Label(Props props, ITransform transform) : base(props, transform) {
        }

        protected override State StartingState => new();

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => false;

        public override BaseRenderResult Render(int width, int height) =>
            new(props.Label.PadRight(width)[..width], Enumerable.Empty<FormattingRange>());

        internal record Props(string Label);
        internal record State();

    }
}
