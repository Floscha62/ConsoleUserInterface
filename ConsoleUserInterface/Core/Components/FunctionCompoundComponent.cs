namespace ConsoleUserInterface.Core.Components;

internal class FunctionCompoundComponent<P, S> : CompoundComponent<FunctionCompoundComponent<P, S>.Props, S> where S : new() {

    private readonly Action<S> setState;
    private readonly Callbacks callbacks = new();

    public FunctionCompoundComponent(FunctionCompoundComponent<P, S>.Props props, ITransform transform) : base(props, transform) {
        if (props.InitialState != null) {
            CurrentState = props.InitialState;
        }
        setState = s => CurrentState = s;
    }

    internal override string TypeName => this.props.ImplementationName ?? base.TypeName;

    internal record Props(P P, S? InitialState, string? ImplementationName, Func<ConsoleKeyInfo, P, S, Action<S>, bool>? HandleKeys, Func<P, S, Action<S>, Callbacks, CompoundRenderResult> Implementation) {
        public override string ToString() => P?.ToString() ?? "";
    }

    public override bool ReceiveKey(ConsoleKeyInfo keyInfo) =>
        props.HandleKeys?.Invoke(keyInfo, props.P, CurrentState, setState) ?? false;

    public override CompoundRenderResult Render() {
        callbacks.Reset();
        return props.Implementation(props.P, CurrentState, setState, callbacks);
    }

}
