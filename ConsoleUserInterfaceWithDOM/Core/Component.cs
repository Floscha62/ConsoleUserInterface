using ConsoleUserInterfaceWithDOM.Core.Dom;

namespace ConsoleUserInterfaceWithDOM.Core {
    public abstract class Component<P, S> : IComponent where S : new() {
        public ITransform Transform => transform;
        public IPropsAndState PropsAndState => new PropsAndState<P, S>(props, state);

        protected readonly ITransform transform;
        protected readonly P props;
       
        public event Action? OnStateChanged;
        protected S CurrentState { get => state; set { state = value; OnStateChanged?.Invoke(); } }
        private S state;

        public Component(P props, ITransform transform) {
            this.transform = transform;
            this.props = props;
            this.state = new();
        }

        public abstract bool ReceiveKey(ConsoleKeyInfo keyInfo);
    }
}
