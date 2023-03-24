using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUserInterface.Core.Extensions;

namespace ConsoleUserInterface.Core.Components {
    internal class ListSelection<T> : CompoundComponent<ListSelection<T>.Props, ListSelection<T>.State> {

        public ListSelection(Props props, ITransform transform) : base(props, transform) {
        }

        protected override State StartingState => new(false, props.StartIndex);

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) {
            switch (keyInfo) {
                case ConsoleKeyInfo(ConsoleKey.Enter, _, _):
                    if (state.Open) props.OnSelect(props.Values[state.SelectedIndex]);
                    state = state with { Open = !state.Open };
                    return true;
                case ConsoleKeyInfo(ConsoleKey.UpArrow, _, _) when state.Open && state.SelectedIndex > 0:
                    state = state with { SelectedIndex = state.SelectedIndex - 1 };
                    return true;
                case ConsoleKeyInfo(ConsoleKey.DownArrow, _, _) when state.Open && state.SelectedIndex < props.Values.Count - 1:
                    state = state with { SelectedIndex = state.SelectedIndex + 1 };
                    return true;
            }
            return false;
        }

        public override CompoundRenderResult Render(int width, int height, bool inFocus) => new(RenderInternal(inFocus));

        private IEnumerable<(IComponent, bool)> RenderInternal(bool inFocus) {
            yield return (Components.Label(Transform, props.LabelFunc(props.Values[state.SelectedIndex])), !state.Open && inFocus);

            var modal = Components.Modal(
                this.Transform,
                Components.Container(
                    ITransform.Create(props.Values.Select(props.LabelFunc).Max(s => s.Length) + 3, props.DisplayedSize),
                    Layout.VERTICAL,
                    props.Values.Select((v, i) => Components.Label(
                        ITransform.Create(1), 
                        i == state.SelectedIndex ? $"> {props.LabelFunc(v)}" : $"  {props.LabelFunc(v)}", 
                        underlined: i == state.SelectedIndex)
                    ).Skip(Math.Min(state.SelectedIndex, props.Values.Count - props.DisplayedSize)).Take(props.DisplayedSize)
                )
            );
            if (state.Open) {
                yield return (modal, inFocus);
            }
        }

        internal record Props(List<T> Values, Func<T, string> LabelFunc, Action<T> OnSelect, int StartIndex = 0, int DisplayedSize = 50);
        internal record State(bool Open, int SelectedIndex);
    }
}
