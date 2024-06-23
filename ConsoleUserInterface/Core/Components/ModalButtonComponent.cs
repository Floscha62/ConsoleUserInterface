namespace ConsoleUserInterface.Core.Components;

internal static class ModalButtonComponent {

    record Props(ITransform ModalTransform, bool ModalChildrenFocusable, List<IComponent> ModalContent, string Label) {
        public override string ToString() => $"Props {{ Label = {Label} }}";

        public virtual bool Equals(Props? props) =>
            props is not null &&
            Equals(ModalTransform, props.ModalTransform) &&
            ModalChildrenFocusable == props.ModalChildrenFocusable &&
            Enumerable.SequenceEqual(ModalContent, props.ModalContent) &&
            Label == props.Label;

        public override int GetHashCode() =>
            HashCode.Combine(ModalTransform, ModalChildrenFocusable, ModalContent, Label);
    }
    record State(bool Open) { 
        public State() : this(false) { }

    }
    internal static IComponent ModalButton(ITransform transform, ITransform modalTransform, bool modalChildrenFocusable, List<IComponent> modalContent, string label) =>
        Components.FunctionComponent<Props, State>(transform, new(modalTransform, modalChildrenFocusable, modalContent, label), ModalButton);

    static CompoundRenderResult ModalButton(Props props, State state, Action<State> updateState, Callbacks _) {
        IEnumerable<IComponent> Comps() {
            yield return Components.Button(ITransform.Create(), props.Label, () => updateState(state with { Open = !state.Open }));
            if (state.Open) {
                yield return Components.Modal(props.ModalTransform, props.ModalChildrenFocusable, props.ModalContent);
            }
        }

        return new(Comps(), SelfFocusable: false);
    }
}
