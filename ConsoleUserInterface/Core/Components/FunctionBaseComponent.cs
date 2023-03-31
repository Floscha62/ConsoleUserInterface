namespace ConsoleUserInterface.Core.Components {
    internal class FunctionBaseComponent<P, S> : BaseComponent<FunctionBaseComponent<P, S>.Props, S> where S : new() {
        public FunctionBaseComponent(FunctionBaseComponent<P, S>.Props props, ITransform transform) : base(props, transform) {
            if (props.InitialState != null) {
                CurrentState = props.InitialState;
            }
        }

        internal override string TypeName => this.props.ImplementationName ?? base.TypeName;

        internal record Props(P P, S? InitialState, string? ImplementationName, Func<ConsoleKeyInfo, P, S, Action<S>, bool>? HandleKeys, Func<P, S, Action<S>, BaseRenderResult> Implementation) {
            public override string ToString() => P?.ToString() ?? "";
        }

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) =>
            props.HandleKeys?.Invoke(keyInfo, props.P, CurrentState, s => CurrentState = s) ?? false;

        public override BaseRenderResult Render() => props.Implementation(props.P, CurrentState, s => CurrentState = s);

    }
}
