using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core {
    internal class Container : CompoundComponent<Container.Props, Container.State> {
        public Layout Layout { get; }
        protected override State StartingState => new(0);

        public Container(Props props, ITransform transform) : base(props, transform) {
            Layout = props.Layout;
        }

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) {
            var comps = props.Components.ToArray();
            state = state.SelectedElement < comps.Length ? state : state with { SelectedElement = comps.Length };

            if (keyInfo.Key == ConsoleKey.Tab) {
                state = state with { SelectedElement = (state.SelectedElement + 1) % (comps.Length + 1) };
                return state.SelectedElement > 0;
            }
            if (state.SelectedElement > 0) {
                return comps[state.SelectedElement - 1].ReceiveKey(keyInfo);
            }
            return false;
        }

        public override CompoundRenderResult Render(int width, int height) =>
            new(props.Components, Enumerable.Empty<FormattingRange>());

        internal record Props(Layout Layout, IEnumerable<IComponent> Components);
        internal record State(int SelectedElement);
    }
}
