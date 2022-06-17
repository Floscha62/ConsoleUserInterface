using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUserInterface.Core.Extensions;
using static ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Core.Components {
    internal class Form : VerticalComponent<Form.Props, Form.State> {
        internal record Props(IEnumerable<(string label, IComponent component)> Elements);
        internal record State(int Focused);

        protected override State StartingState => new State(0);

        public Form(Props props) : base(props) { }

        public override bool ReceiveKey(ConsoleKeyInfo key) {
            switch (key) {
                case ConsoleKeyInfo(ConsoleKey.UpArrow, _, 0) when props.Elements.ElementAt(state.Focused).component.ReceiveKey(key):
                    return true;
                case ConsoleKeyInfo(ConsoleKey.UpArrow, _, _) when state.Focused > 0:
                    state = state with { Focused = state.Focused - 1 };
                    return true;
                case ConsoleKeyInfo(ConsoleKey.DownArrow, _, 0) when props.Elements.ElementAt(state.Focused).component.ReceiveKey(key):
                    return true;
                case ConsoleKeyInfo(ConsoleKey.DownArrow, _, _) when state.Focused < props.Elements.Count() - 1:
                    state = state with { Focused = state.Focused + 1 };
                    return true;
                case var k:
                    return props.Elements.ElementAt(state.Focused).component.ReceiveKey(k);
            }
        }

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() =>
            (props.Elements.SelectMany((t, i) => new[] {
                Label(t.label, i == state.Focused),
                Label(""),
                t.component,
                Label("")
            }), Enumerable.Empty<FormattingRange>());

    }
}
