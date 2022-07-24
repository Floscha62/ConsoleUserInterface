using System;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class Label : BaseComponent<Label.Props, Label.State> {

        public Label(Props props, ITransform transform) : base(props, transform) {
        }

        protected override State StartingState => new();

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => false;

        public override BaseRenderResult Render(int width, int height) =>
            new(props.Label.PadRight(width)[..width], props.Underlined ? new[] { 
                IFormatting.Underline((0, 0), (width - 1, 0))
            }: Enumerable.Empty<FormattingRange>());

        internal record Props(string Label, bool Underlined);
        internal record State();

    }
}
