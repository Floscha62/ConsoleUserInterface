using ConsoleUserInterface.Core.Extensions;
using System;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class Label : BaseComponent<Label.Props, Label.State> {

        internal record Props(string Label, string Ellipsis, bool Underlined);
        internal record State();

        public Label(Props props, ITransform transform) : base(props, transform) {
        }

        protected override State StartingState => new();

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => false;

        public override BaseRenderResult Render(int width, int height) =>
            new(props.Label.Ellipsis(props.Ellipsis, width).PadRight(width)[..width], props.Underlined ? new[] { 
                IFormatting.Underline((0, 0), (Math.Min(props.Label.Length, width) - 1, 0))
            }: Enumerable.Empty<FormattingRange>());


    }
}
