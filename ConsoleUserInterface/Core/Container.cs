using System;
using System.Collections.Generic;
using System.Linq;
using static ConsoleUserInterface.Core.Utils.FunctionUtils;

namespace ConsoleUserInterface.Core {
    internal class Container : CompoundComponent<Container.Props, Container.State> {
        public Layout Layout { get; }
        protected override State StartingState => new(0);

        public Container(Props props, ITransform transform) : base(props, transform) {
            Layout = props.Layout;
        }

        private bool ChildReceiveKey(IComponent[] comps, ConsoleKeyInfo keyInfo) => 
            state.SelectedElement > 0 && comps[state.SelectedElement - 1].ReceiveKey(keyInfo);

        private bool FocusNext(IComponent[] comps, ConsoleKeyInfo keyInfo) {
            if (keyInfo.Key == ConsoleKey.Tab) {
                state = state with { SelectedElement = (state.SelectedElement + 1) % (comps.Length + 1) };
                return state.SelectedElement > 0;
            }
            return false;
        }

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) {
            var comps = props.Components.ToArray();
            state = state.SelectedElement < comps.Length ? state : state with { SelectedElement = comps.Length };

            return Any(
                comps, 
                keyInfo,
                ChildReceiveKey,
                FocusNext
            );
        }

        public override CompoundRenderResult Render(int width, int height, bool inFocus) =>
            new(props.Components.Zip(props.Components.Select((_, i) => inFocus && i + 1 == state.SelectedElement)));

        internal record Props(Layout Layout, IEnumerable<IComponent> Components);
        internal record State(int SelectedElement);
    }
}
