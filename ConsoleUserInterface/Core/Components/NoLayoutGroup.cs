using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class NoLayoutGroup : NoLayoutComponent<NoLayoutGroup.Props, NoLayoutGroup.State> {
        internal record Props(IEnumerable<IComponent> Components);
        internal record State();

        public NoLayoutGroup(Props props) : base(props) { }

        public override bool ReceiveKey(System.ConsoleKeyInfo key) {
            foreach (var child in props.Components) {
                if (child.ReceiveKey(key)) {
                    return true;
                }
            }
            return false;
        }

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() =>
            (props.Components, Enumerable.Empty<FormattingRange>());

        protected override State StartingState => new State();
    }
}
