namespace ConsoleUserInterface.Core.Components;
internal class FunctionBaseComponent<P, S> : BaseComponent<FunctionBaseComponent<P, S>.Props, S> where S : new() {

    private readonly Action<Func<S, S>> setState;
    private readonly Callbacks callbacks = new();

    public FunctionBaseComponent(FunctionBaseComponent<P, S>.Props props, ITransform transform) : base(props, transform) {
        if (props.InitialState != null) {
            CurrentState = props.InitialState;
        }
        setState = s => CurrentState = s(CurrentState);
    }

    internal override string TypeName => this.props.ImplementationName ?? base.TypeName;

    internal record Props(P P, S? InitialState, string? ImplementationName, Func<ConsoleKeyInfo, P, S, Action<Func<S,S>>, bool>? HandleKeys, Func<P, S, Action<Func<S, S>>, Callbacks, BaseRenderResult> Implementation) {
        public override string ToString() => P?.ToString() ?? "";
    }

    public override bool ReceiveKey(ConsoleKeyInfo keyInfo) =>
        props.HandleKeys?.Invoke(keyInfo, props.P, CurrentState, setState) ?? false;

    public override BaseRenderResult Render() => props.Implementation(props.P, CurrentState, setState, callbacks);

}
