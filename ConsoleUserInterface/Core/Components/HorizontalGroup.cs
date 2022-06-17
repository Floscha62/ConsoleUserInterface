using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUserInterface.Core.Extensions;

namespace ConsoleUserInterface.Core.Components {
    internal class HorizontalGroup : HorizontalComponent<HorizontalGroup.Props, HorizontalGroup.State> {
        internal record Props(IEnumerable<IComponent> Components);
        internal record State(int FocusedIndex);

        public HorizontalGroup(Props props) : base(props) { }

        public override bool ReceiveKey(ConsoleKeyInfo key) {
            if (props.Components.ElementAt(state.FocusedIndex).ReceiveKey(key)) {
                return true;
            }

            switch (key) {
                case ConsoleKeyInfo(ConsoleKey.Tab, _, _):
                    state = state with { FocusedIndex = (state.FocusedIndex + 1) % props.Components.Count() };
                    return true;
            }

            return false;
        }

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() =>
            (props.Components, Enumerable.Empty<FormattingRange>());

        protected override State StartingState => new State(0);
    }
}
