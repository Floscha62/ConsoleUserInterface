using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class CenteredComponent : Component<CenteredComponent.Props, CenteredComponent.State>, IConstOffsetComponent {
        internal record Props(IComponent ToCenter);
        internal record State();

        public override int Height => 0;
        public override int Width => 0;
        public (int x, int y) Offset(IConsole console) => 
            (console.WindowWidth / 2 - props.ToCenter.Width / 2, console.WindowHeight / 2 - props.ToCenter.Height / 2);

        public CenteredComponent(Props props, int layerIndex = 0) : base(props, layerIndex) { }

        public IComponent RenderComponent() => props.ToCenter;

        public override bool ReceiveKey(ConsoleKeyInfo key) => props.ToCenter.ReceiveKey(key);

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() => (new[] { props.ToCenter }, Enumerable.Empty<FormattingRange>());

        protected override State StartingState => new State();
    }
}
