using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class Container : Component<Container.Props, Container.State>, IVerticalComponent {
        internal record Props(IComponent Component, int Width, int Height, bool Focussed);
        internal record State();

        public override int Width => props.Width;
        public override int Height => props.Height;

        internal Container(Props props, int layerIndex = 0) : base(props, layerIndex) { }

        public override bool ReceiveKey(ConsoleKeyInfo key) => props.Component.ReceiveKey(key);

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() => (
            new[] { props.Component },
            props.Focussed ? 
                Enumerable.Range(0, Height).Select(i => IFormatting.Background(150, 150, 150, (0, i), (Width - 1, i))) : 
                Enumerable.Empty<FormattingRange>()
            );

        protected override State StartingState => new State();

    }
}
