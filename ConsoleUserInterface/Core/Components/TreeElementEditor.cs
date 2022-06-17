using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUserInterface.Core.Extensions;
using static ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Core.Components {
    internal class TreeElementEditor<T> : HorizontalComponent<TreeElementEditor<T>.Props, TreeElementEditor<T>.State> where T : TreeElement<T> {
        internal record Props(T Root, Func<T, IComponent> Func);
        internal record State(IComponent Tree, IComponent CurrentView, bool OnTree);

        protected override State StartingState => new State(TreeView(
                props.Root,
                t => {
                    state = state with { CurrentView = props.Func(t) };
                }), props.Func(props.Root), true);

        public TreeElementEditor(Props props) : base(props) { }

        public override bool ReceiveKey(ConsoleKeyInfo key) {
            switch (key) {
                case ConsoleKeyInfo(ConsoleKey.Tab, _, _):
                    state = state with { OnTree = !state.OnTree };
                    return true;
            }

            return state.OnTree ? state.Tree.ReceiveKey(key) : state.CurrentView.ReceiveKey(key);
        }

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() =>
            (RenderInternal(), Enumerable.Empty<FormattingRange>());

        IEnumerable<IComponent> RenderInternal() {
            yield return Group(Layout.HORIZONTAL, state.Tree,
                Group(Layout.VERTICAL, Enumerable.Repeat(Label("|"),
                    Math.Max(state.Tree.Height, props.Root.GetChildrenRecursive().Select(props.Func).Select(c => c.Width).Max()))
                ),
                state.CurrentView);
        }
    }
}
