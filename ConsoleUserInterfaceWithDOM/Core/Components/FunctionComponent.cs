namespace ConsoleUserInterfaceWithDOM.Core.Components {
    internal class FunctionComponent<P, S> : CompoundComponent<FunctionComponent<P, S>.Props, FunctionComponent<P, S>.State> where S : new() {
        public FunctionComponent(FunctionComponent<P, S>.Props props, ITransform transform) : base(props, transform) {
            if (props.InitialState != null) {
                CurrentState = new(props.InitialState, null);
            }
        }

        internal record Props(P P, S? InitialState, Func<P, S, Action<S>, (Func<ConsoleKeyInfo, Action<S>, bool>?, CompoundRenderResult)> Implementation);
        internal record State(S S, Func<ConsoleKeyInfo, Action<S>, bool>? KeyHandler) { public State() : this(new(), null) { } }

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => 
            CurrentState.KeyHandler?.Invoke(keyInfo, s => CurrentState = CurrentState with { S = s }) ?? false;

        public override CompoundRenderResult Render() {
            var (keyHandler, result) =  props.Implementation(props.P, CurrentState.S, s => CurrentState = CurrentState with { S = s });
            if(CurrentState.KeyHandler == null) {
                CurrentState = CurrentState with { KeyHandler = keyHandler };
            }
            return result;
        }

    }
}
