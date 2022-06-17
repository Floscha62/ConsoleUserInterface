using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class VerticalGroup : VerticalComponent<VerticalGroup.Props, VerticalGroup.State> {
        internal record Props(IEnumerable<IComponent> Components);
        internal record State();

        public VerticalGroup(Props props) : base(props) { }

        public override bool ReceiveKey(System.ConsoleKeyInfo key) {
            foreach (var child in props.Components) {
                if (child.ReceiveKey(key)) {
                    return true;
                }
            }
            return false;
        }

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() =>
            (RenderInternal(), Enumerable.Empty<FormattingRange>());

        IEnumerable<IComponent> RenderInternal() =>
            props.Components;

        protected override State StartingState => new State();
    }
}
