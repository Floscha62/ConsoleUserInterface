using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    internal class ListSelection<T> : CompoundComponent<ListSelection<T>.Props, ListSelection<T>.State> {
        private readonly IComponent modal;

        public ListSelection(Props props, ITransform transform) : base(props, transform) {
            modal = Components.Modal(
                transform,
                ListElements()
            );
        }

        private IEnumerable<IComponent> ListElements() {
            var i = 0;
            foreach (var value in props.Values) {
                var label = props.LabelFunc(value);
                var selected = i == state.SelectedIndex;
                yield return Components.Label(ITransform.Create(1), label, underlined: i == state.SelectedIndex);
                i++;
            }
        }

        protected override State StartingState => new(false, props.StartIndex);

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) {
            throw new NotImplementedException();
        }

        public override CompoundRenderResult Render(int width, int height, bool inFocus) => new CompoundRenderResult(RenderInternal(inFocus));

        private IEnumerable<(IComponent, bool)> RenderInternal(bool inFocus) {
            yield return (Components.Label(Transform, props.LabelFunc(props.Values[state.SelectedIndex])), !state.Open && inFocus);
            if (state.Open) yield return (modal, inFocus);
        }

        internal record Props(List<T> Values, Func<T, string> LabelFunc, int StartIndex = 0);
        internal record State(bool Open, int SelectedIndex);
    }
}
