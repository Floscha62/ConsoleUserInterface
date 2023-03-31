namespace ConsoleUserInterface.Core.Components {
    internal class FunctionCompoundComponent<P, S> : CompoundComponent<FunctionCompoundComponent<P, S>.Props, S> where S : new() {
        public FunctionCompoundComponent(FunctionCompoundComponent<P, S>.Props props, ITransform transform) : base(props, transform) {
            if (props.InitialState != null) {
                CurrentState = props.InitialState;
            }
        }

        internal override string TypeName => this.props.ImplementationName ?? base.TypeName;

        internal record Props(P P, S? InitialState, string? ImplementationName, Func<ConsoleKeyInfo, P, S, Action<S>, bool>? HandleKeys, Func<P, S, Action<S>, CompoundRenderResult> Implementation) {
            public override string ToString() => P?.ToString() ?? "";
        }

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) =>
            props.HandleKeys?.Invoke(keyInfo, props.P, CurrentState, s => CurrentState = s) ?? false;

        public override CompoundRenderResult Render() => props.Implementation(props.P, CurrentState, s => CurrentState = s);

    }
}
